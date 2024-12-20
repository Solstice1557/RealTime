using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealTime.BL.Alphavantage;
using RealTime.BL.Sync;
using RealTime.DAL;
using RealTime.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.PortfolioTest
{
    public class Program
    {
        private static ILogger _logger;

        private static readonly Dictionary<long, Dictionary<DateTime, DailyPrice>> DailyPricesCache
            = new Dictionary<long, Dictionary<DateTime, DailyPrice>>();

        private static readonly Dictionary<long, decimal> MarketCapCache = new Dictionary<long, decimal>();

        public static async Task Main()
        {
            var serviceProvider = Initialization.InitServices();

            _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => _logger.LogCritical("Unhandled exception", (Exception)e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (s, e) => _logger.LogError("Unhandled task exception", e.Exception);

            await Sync(serviceProvider, CancellationToken.None);
            var db = serviceProvider.GetService<PricesDbContext>();
            var alphavantageService = serviceProvider.GetService<IAlphavantageService>();

            var startDate = new DateTime(2017, 1, 1);
            var dates = Enumerable.Range(0, 37).Select(i => startDate.AddMonths(i)).ToArray();

            await ComparePortfolios(db, dates, alphavantageService);

            _logger.LogInformation("Finish. Press any key.");
            Console.ReadKey();
        }

        private static async Task ComparePortfolios(PricesDbContext db, DateTime[] dates, IAlphavantageService alphavantageService)
        {
            var notSupportedFunds = new[] {
                389, 98, 314, 261, 224, 515, 134, 502, 290, 303, 503, // stocks does not contain all the prices for 2019
                517, 40, 132, 170, 317, 321, 349, 414, // stocks market capitalization could not be found
                4, // duplicates
                443, 492, 512, // stocks does not not contain all the prices for 2017 and 2018
                7, 67, 172, 271 // error in prices
            };
            var allFunds = await db.Funds.Where(x => !notSupportedFunds.Contains(x.FundId)).ToListAsync();

           /* var symbols = "AMZN,INTU,FB,MSFT,AAPL,CRM,NFLX,NVDA,AMD,F".Split(',');
            allFunds = allFunds.Where(x => symbols.Contains(x.Symbol)).ToList();*/

            await LoadPricesCache(db, dates, allFunds);

            await LoadMarketCapCache(allFunds, dates[0], alphavantageService);

            var rnd = new Random((int)DateTime.UtcNow.Ticks);

            // generating portfolios and compare performance
            var differencesList = new List<Dictionary<DateTime, PerformanceDifference>>();
            for (var i = 0; i < 1000; i++)
            {
                var funds = allFunds.OrderBy(_ => rnd.Next()).Take(10).ToList();
                Console.WriteLine($"Selected funds: {string.Join(",", funds.Select(x => x.Symbol))}");
                var fundsIds = funds.Select(x => x.FundId).ToList();
                var portfolio1 = GetPositionsEvenlyAllocated(fundsIds);
                var portfolio2 = GetPositionsAllocatedAccordinglyCapitalization(fundsIds);

                var tt = portfolio2.Sum(x => x.Share);

                var performance1 = CalculatePositionsPerformance(dates, portfolio1);
                var performance2 = CalculatePositionsPerformance(dates, portfolio2);
                var diffrerences = performance1.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new PerformanceDifference(kvp.Value, performance2[kvp.Key]));

                PrintPerformance(diffrerences);
                differencesList.Add(diffrerences);
            }

            // calulating mean values of performances
            var meanPerformance = GetMeanPerformance(dates, differencesList);

            Console.WriteLine();
            Console.WriteLine("Mean performance");
            PrintPerformance(meanPerformance);
        }

        private static Dictionary<DateTime, PerformanceDifference> GetMeanPerformance(
            DateTime[] dates,
            List<Dictionary<DateTime, PerformanceDifference>> differencesList)
        {
            var p1MeanValues = dates.ToDictionary(x => x, _ => 0m);
            var p2MeanValues = dates.ToDictionary(x => x, _ => 0m);
            foreach (var difference in differencesList)
            {
                foreach (var date in dates)
                {
                    p1MeanValues[date] += difference[date].P1Performance;
                    p2MeanValues[date] += difference[date].P2Performance;
                }
            }

            var meanValueDif = new Dictionary<DateTime, PerformanceDifference>();
            foreach (var date in dates)
            {
                var dif = new PerformanceDifference(
                    p1MeanValues[date] / differencesList.Count,
                    p2MeanValues[date] / differencesList.Count);
                meanValueDif.Add(date, dif);
            }

            return meanValueDif;
        }

        private static Position[] GetPositionsEvenlyAllocated(ICollection<int> fundIds)
        {
            return fundIds.Select(
                    x => new Position
                    {
                        FundId = x,
                        Share = 1.0m / fundIds.Count
                    })
                    .ToArray();
        }

        private static Position[] GetPositionsAllocatedAccordinglyCapitalization(ICollection<int> fundIds)
        {
            const int coefficient = 1000000000;
            var totalCapitalization = fundIds.Sum(x => coefficient / MarketCapCache[x]);
            return fundIds.Select(
                    x => new Position
                    {
                        FundId = x,
                        Share = coefficient / (MarketCapCache[x] * totalCapitalization)
                    })
                    .ToArray();
        }

        private static async Task LoadMarketCapCache(List<Fund> funds, DateTime priceDate, IAlphavantageService alphavantageService)
        {
            _logger.LogInformation("Start loading market capitaliztion.");
            foreach (var fund in funds)
            {
                var price = DailyPricesCache[fund.FundId][priceDate].Close.Value;
                
                var info = await alphavantageService.GetFundOverview(fund.Symbol);
                if (info == null)
                {
                    _logger.LogError($"Market capitaliztion for {fund.Symbol} not found.");
                }
                else if (info.SharesOutstanding <= 0)
                {
                    _logger.LogError($"Market capitaliztion for {fund.Symbol} error: SharesOutstanding = {info.SharesOutstanding}.");
                }
                else
                {
                    var marketCap = price * info.SharesOutstanding;
                    _logger.LogInformation($"Market capitaliztion for {fund.Symbol} = {marketCap:F02}.");
                    MarketCapCache.Add(fund.FundId, marketCap);
                }
            }

            _logger.LogInformation("Finish loading market capitaliztion.");
        }

        private static async Task LoadPricesCache(PricesDbContext db, DateTime[] dates, List<Fund> funds)
        {
            _logger.LogInformation("Start loading prices cache.");
            foreach (var fund in funds)
            {
                if (DailyPricesCache.ContainsKey(fund.FundId))
                {
                    continue;
                }

                var prices = new Dictionary<DateTime, DailyPrice>();
                foreach (var date in dates)
                {
                    var dailyPrice = await db.DailyPrices
                        .Where(x => x.FundId == fund.FundId && x.Timestamp <= date)
                        .OrderByDescending(x => x.Timestamp)
                        .FirstOrDefaultAsync();

                    if (dailyPrice == null)
                    {
                        _logger.LogError($"No prices for {fund.Symbol} {fund.FundId} on {date:yyyy-MM-dd}");
                    }
                    else if (dailyPrice.Timestamp < date.AddDays(-14))
                    {
                        _logger.LogError($"Prices for {fund.Symbol} {fund.FundId} on {date:yyyy-MM-dd} is outdated ({dailyPrice.Timestamp:yyyy-MM-dd})");
                    }
                    else
                    {
                        prices.Add(date, dailyPrice);
                    }
                }

                DailyPricesCache.Add(fund.FundId, prices);
            }

            _logger.LogInformation("Finish loading prices cache.");
        }

        private static async Task Sync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start syncing daily data");
            var syncronizer = serviceProvider.GetService<IPricesSyncronizer>();
            await syncronizer.SyncDailyPrices(cancellationToken);
            _logger.LogInformation("Finish syncing daily data");
        }

        private static Dictionary<DateTime, decimal> CalculatePositionsPerformance(
            DateTime[] dates,
            Position[] positions)
        {
            var initialPrices = new Dictionary<long, decimal>();
            foreach (var position in positions)
            {
                var fundsPrices = DailyPricesCache[position.FundId];
                initialPrices.Add(position.FundId, fundsPrices[dates[0]].Close.Value);
            }

            var result = new Dictionary<DateTime, decimal>();
            result.Add(dates[0], 0);

            for (int i = 1; i < dates.Length; i++)
            {
                var date = dates[i];
                decimal sum = 0;
                foreach (var position in positions)
                {
                    var initialPrice = initialPrices[position.FundId];
                    var currentPrice = DailyPricesCache[position.FundId][date].Close.Value;

                    var priceChange = (currentPrice - initialPrice) / initialPrice;
                    if (priceChange > 20)
                    {
                        throw new Exception("Invalid price change");
                    }

                    sum += priceChange * position.Share;
                }

                result.Add(date, sum);
            }

            return result;
        }

        private static void PrintPerformance(Dictionary<DateTime, PerformanceDifference> diffrerences)
        {
            Console.WriteLine("Date\t\t| P1\t\t| P2\t\t| Difference\t|");
            Console.WriteLine("_________________________________________________________________");
            foreach (var kvp in diffrerences)
            {
                var dif = kvp.Value.Dif;
                var difStr = dif.HasValue ? (FormatPercent(dif.Value) + "%") : "       ";
                var p1ValStr = FormatPercent(kvp.Value.P1Performance);
                var p2ValStr = FormatPercent(kvp.Value.P2Performance);

                Console.WriteLine($"{kvp.Key:yyyy.MM.dd}\t| {p1ValStr}%\t| {p2ValStr}%\t| {difStr}\t|");
            }

            Console.WriteLine("_________________________________________________________________");
        }

        private static string FormatPercent(decimal val)
        {
            var str = (val * 100).ToString("F02");
            while (str.Length < 7)
            {
                str = " " + str;
            }

            return str;
        }
    }

    public class PerformanceDifference
    {
        private const decimal Epsilon = 0.0001m;

        public decimal P1Performance { get; }

        public PerformanceDifference(decimal p1Performance, decimal p2Performance)
        {
            P1Performance = p1Performance;
            P2Performance = p2Performance;
        }

        public decimal P2Performance { get; }

        public decimal? Dif => P1Performance < Epsilon ? (decimal?)null : (P2Performance - P1Performance) / P1Performance;
    }

    public class Position
    {
        public long FundId { get; set; }

        public decimal Share { get; set; }
    }
}
