using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealTime.DAL;
using RealTime.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.PriceChangeMonitoring
{
    public class PriceMonotoringService
    {
        private const int TopCount = 20;
        private readonly static TimeSpan MonitoringTimespan = TimeSpan.FromMinutes(5);

        private readonly IServiceProvider serviceProvider;
        private readonly TradingService tradingService;
        private readonly TimeZoneInfo marketTimeZone;

        private IReadOnlyDictionary<int, string> symbolDictionary;
        private IReadOnlyDictionary<int, decimal> startDayPrices = new Dictionary<int, decimal>();
        private IReadOnlyDictionary<int, decimal> yesterdayClosePrices = new Dictionary<int, decimal>();
        private IReadOnlyDictionary<int, decimal> previousPrices = new Dictionary<int, decimal>();
        private DateTime startDayPricesTime;

        public PriceMonotoringService(IServiceProvider serviceProvider, TradingService tradingService)
        {
            this.serviceProvider = serviceProvider;
            this.tradingService = tradingService;
            this.marketTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public async Task Monitor(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PricesDbContext>();
                this.symbolDictionary = await db.Funds.ToDictionaryAsync(x => x.FundId, x => x.Symbol);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await CheckPrices();
                    await Task.Delay(MonitoringTimespan, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }

        private async Task CheckPrices()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PricesDbContext>();

            var startDayPrices = await GetStartDayPrices(db);
            var finishYesterdayPrices = await GetFinishYesterdayPrices(db);
            var currentPrices = await GetCurrentPrices(db);

            var startDayPerformance = GetTopPerformance(startDayPrices, currentPrices);
            var finishYesterdayPerformance = GetTopPerformance(finishYesterdayPrices, currentPrices);
            var fiveMinsPerformance = GetTopPerformance(this.previousPrices, currentPrices);

            LogPerformances(finishYesterdayPerformance, startDayPerformance, fiveMinsPerformance);
            if (startDayPerformance.Count > 0 
                || fiveMinsPerformance.Count > 0
                || finishYesterdayPerformance.Count > 0)
            {
                await tradingService.NewPrices(
                    finishYesterdayPerformance,
                    startDayPerformance,
                    fiveMinsPerformance);
            }

            this.previousPrices = currentPrices;
            sw.Stop();
            Console.WriteLine($"Calculated for {sw.Elapsed.TotalSeconds} seconds");
        }

        private IReadOnlyList<Performance> GetTopPerformance(
            IReadOnlyDictionary<int, decimal> basePrices,
            IReadOnlyDictionary<int, decimal> currentPrices)
        {
            var preformances = new List<Performance>(basePrices.Count);
            foreach (var (fundId, basePrice) in basePrices)
            {
                if (!currentPrices.ContainsKey(fundId))
                {
                    continue;
                }

                var currentPrice = currentPrices[fundId];
                var preformance = (currentPrice - basePrice) / basePrice * 100;
                preformances.Add(
                    new Performance(symbolDictionary[fundId], preformance, basePrice, currentPrice));
            }

            return preformances.OrderByDescending(x => x.PerformanceInPercent)
                .Take(TopCount)
                .ToList();
        }

        private void LogPerformances(
            IReadOnlyList<Performance> finishYesterdayPerformance,
            IReadOnlyList<Performance> startDayPerformance,
            IReadOnlyList<Performance> minutePerformances)
        {
            Console.WriteLine("Yesterday\t\t\tDay\t\t\t5 MINS");
            var index = finishYesterdayPerformance.Count;
            if (index < startDayPerformance.Count)
            {
                index = startDayPerformance.Count;
            }

            if (index < minutePerformances.Count)
            {
                index = minutePerformances.Count;
            }

            if (index == 0)
            {
                return;
            }

            var marketNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.marketTimeZone);
            var currentDateStr = marketNow.ToString("HH:mm:ss dd MMM yyyy", CultureInfo.InvariantCulture);
            var sb = new StringBuilder(currentDateStr + "\n");
            for (var i = 0; i < index; i++)
            {
                var yesterdayPerf = finishYesterdayPerformance.Count > i ? finishYesterdayPerformance[i] : null;
                var dayPerf = startDayPerformance.Count > i ? startDayPerformance[i] : null;
                var minutePerf = minutePerformances.Count > i ? minutePerformances[i] : null;

                Console.WriteLine(
                    "{0}\t\t{1:0.00}%\t\t{2}\t\t{3:0.00}%\t\t{4}\t\t{5:0.00}%",
                    yesterdayPerf?.Symbol ?? "   ",
                    yesterdayPerf?.PerformanceInPercent,
                    dayPerf?.Symbol ?? "   ",
                    dayPerf?.PerformanceInPercent,
                    minutePerf?.Symbol ?? "   ",
                    minutePerf?.PerformanceInPercent);
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0},{1:0.00}%,{2},{3:0.00}%,{4},{5:0.00}%\n",
                    yesterdayPerf?.Symbol ?? "",
                    yesterdayPerf?.PerformanceInPercent,
                    dayPerf?.Symbol ?? "",
                    dayPerf?.PerformanceInPercent,
                    minutePerf?.Symbol ?? "",
                    minutePerf?.PerformanceInPercent);
            }

            var fileName = $"{marketNow.Year}-{marketNow.Month}-{marketNow.Day}.csv";
            if (!File.Exists(fileName))
            {
                sb.Insert(0, "Yesterday,,Day,,5 mins\nSymbol,Perf,Symbol,Perf,Symbol,Perf\n");
            }

            File.AppendAllText(fileName, sb.ToString());
        }

        private async Task<IReadOnlyDictionary<int, decimal>> GetStartDayPrices(PricesDbContext db)
        {
            var marketDate = GetMarketDateUtc();
            if (marketDate != startDayPricesTime || startDayPrices.Count == 0)
            {
                var pricesQuery = db.Prices
                    .Where(x => x.Timestamp > marketDate
                                && x.Close != null);

                startDayPrices = await GetPrices(pricesQuery, false);
                startDayPricesTime = marketDate;
            }

            return startDayPrices;
        }

        private async Task<IReadOnlyDictionary<int, decimal>> GetFinishYesterdayPrices(PricesDbContext db)
        {
            var marketDate = GetMarketDateUtc();
            if (marketDate != startDayPricesTime || yesterdayClosePrices.Count == 0)
            {
                var minPricesDate = marketDate.AddDays(-5);
                var pricesQuery = db.Prices
                    .Where(x => x.Timestamp < marketDate
                                && x.Timestamp > minPricesDate
                                && x.Close != null);

                yesterdayClosePrices = await GetPrices(pricesQuery, true);
                startDayPricesTime = marketDate;
            }

            return yesterdayClosePrices;
        }

        private Task<IReadOnlyDictionary<int, decimal>> GetCurrentPrices(PricesDbContext db)
        {
            var minPriceTimestamp = DateTime.UtcNow.Add(-MonitoringTimespan);
            var pricesQuery = db.Prices
                    .Where(x => x.Timestamp >= minPriceTimestamp && x.Close != null);
            return GetPrices(pricesQuery, true);
        }

        private static async Task<IReadOnlyDictionary<int, decimal>> GetPrices(
            IQueryable<Price> query,
            bool takeLastPrice)
        {
            var prices = await query
                .Select(x => new { x.FundId, x.Close, x.Timestamp })
                .ToArrayAsync();

            return prices
                .GroupBy(x => x.FundId)
                .ToDictionary(
                    g => g.Key,
                    g => takeLastPrice 
                        ? g.OrderBy(x => x.Timestamp).Last().Close
                        : g.OrderBy(x => x.Timestamp).First().Close)
                .Where(x => x.Value.HasValue && x.Value.Value > 0)
                .ToDictionary(x => x.Key, x => x.Value.Value);
        }

        private DateTime GetMarketDateUtc()
        {
            var marketNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.marketTimeZone);
            return TimeZoneInfo.ConvertTimeToUtc(marketNow.Date, this.marketTimeZone);
        }
    }
}
