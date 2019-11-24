namespace RealTime.BL.Sync
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using RealTime.BL.Alphavantage;
    using RealTime.DAL;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class PricesSyncronizer : IPricesSyncronizer
    {
        private readonly IAlphavantageService alphavantageService;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public PricesSyncronizer(
            IAlphavantageService alphavantageService,
            IServiceProvider serviceProvider,
            ILogger<PricesSyncronizer> logger)
        {
            this.alphavantageService = alphavantageService;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task SyncDailyPrices(CancellationToken cancellationToken)
        {
            List<FundToSync> fundsToSync;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                fundsToSync = await dbContext.Funds.OrderByDescending(x => x.Volume)
                    .Select(x => new FundToSync { Id = x.FundId, Symbol = x.Symbol })
                    .ToListAsync(cancellationToken);
            }

            foreach (var fund in fundsToSync)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    this.logger.LogInformation($"Syncing daily for {fund.Symbol}");
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                        await SyncDailyFundPrices(
                            dbContext,
                            alphavantageService,
                            fund.Id,
                            fund.Symbol,
                            cancellationToken);
                    }
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, $"Failed to sync daily prices for {fund.Symbol}");
                }
            }
        }

        public async Task SyncIntradayPrices(CancellationToken cancellationToken)
        {
            List<FundToSync> fundsToSync;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                fundsToSync = await dbContext.Funds.OrderByDescending(x => x.Volume)
                    .Take(120)
                    .Select(x => new FundToSync { Id = x.FundId, Symbol = x.Symbol })
                    .ToListAsync(cancellationToken);
            }

            var firstTime = true;
            var loopTimespan = TimeSpan.FromMinutes(1);
            while (!cancellationToken.IsCancellationRequested)
            {
                var sw = new Stopwatch();
                sw.Start();
                foreach (var fund in fundsToSync)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        this.logger.LogInformation($"Syncing intraday for {fund.Symbol}");
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                            await SyncIntradayFundPrices(
                                dbContext,
                                alphavantageService,
                                fund.Id,
                                fund.Symbol,
                                firstTime,
                                cancellationToken);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        this.logger.LogError(e, $"Failed to sync intraday prices for {fund.Symbol}");
                    }
                }

                sw.Stop();
                if (sw.Elapsed < loopTimespan)
                {
                    await Task.Delay(loopTimespan - sw.Elapsed, cancellationToken);
                }

                firstTime = false;
            }
        }

        private async Task SyncIntradayFundPrices(
            PricesDbContext dbContext,
            IAlphavantageService alphavantageService,
            int id,
            string symbol,
            bool firstTime,
            CancellationToken cancellationToken)
        {
            var prices = await alphavantageService.LoadPricesWithRetry(
                symbol,
                Prices.PricesTimeInterval.Intraday1Min,
                firstTime,
                cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (prices.Count == 0)
            {
                return;
            }

            if (!firstTime)
            {
                prices = prices.OrderByDescending(kp => kp.Key)
                    .Take(10)
                    .ToDictionary(kp => kp.Key, kp => kp.Value);
            }

            var minDate = prices.Keys.Min();
            var maxDate = prices.Keys.Max();
            var exisingPricesTimestamp = dbContext.Prices
                .Where(
                    x => x.FundId == id
                         && x.Timestamp >= minDate
                         && x.Timestamp <= maxDate)
                .Select(x => x.Timestamp)
                .ToList();
            var priceBulkCount = 0;
            foreach (var kp in prices)
            {
                if (exisingPricesTimestamp.Contains(kp.Key))
                {
                    continue;
                }

                priceBulkCount++;
                dbContext.Prices.Add(
                    new DAL.Entities.Price
                    {
                        FundId = id,
                        Timestamp = kp.Key,
                        Close = kp.Value.Close.ToDecimal(),
                        High = kp.Value.High.ToDecimal(),
                        Open = kp.Value.Open.ToDecimal(),
                        Low = kp.Value.Low.ToDecimal(),
                        Volume = kp.Value.Volume.ToDecimal()
                    });

                if (priceBulkCount >= 100)
                {
                    await dbContext.SaveChangesAsync(cancellationToken);
                    priceBulkCount = 0;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task SyncDailyFundPrices(
            PricesDbContext dbContext,
            IAlphavantageService alphavantageService,
            int id,
            string symbol,
            CancellationToken cancellationToken)
        {
            var haveAnyPrices = await dbContext.DailyPrices.AnyAsync(p => p.FundId == id);
            var prices = await alphavantageService.LoadPricesWithRetry(
                symbol,
                Prices.PricesTimeInterval.Daily,
                !haveAnyPrices,
                cancellationToken);
            if (prices.Count == 0)
            {
                return;
            }

            var minDate = prices.Keys.Min();
            var maxDate = prices.Keys.Max();
            var exisingPricesTimestamp = dbContext.DailyPrices
                .Where(
                    x => x.FundId == id
                         && x.Timestamp >= minDate
                         && x.Timestamp <= maxDate)
                .Select(x => x.Timestamp)
                .ToList();
            var priceBulkCount = 0;
            foreach (var kp in prices)
            {
                if (exisingPricesTimestamp.Contains(kp.Key))
                {
                    continue;
                }

                priceBulkCount++;
                dbContext.DailyPrices.Add(
                    new DAL.Entities.DailyPrice
                    {
                        FundId = id,
                        Timestamp = kp.Key,
                        Close = kp.Value.Close.ToDecimal(),
                        High = kp.Value.High.ToDecimal(),
                        Open = kp.Value.Open.ToDecimal(),
                        Low = kp.Value.Low.ToDecimal(),
                        Volume = kp.Value.Volume.ToDecimal()
                    });

                if (priceBulkCount >= 100)
                {
                    await dbContext.SaveChangesAsync(cancellationToken);
                    priceBulkCount = 0;
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private class FundToSync
        {
            public int Id { get; set; }

            public string Symbol { get; set; }
        }
    }
}
