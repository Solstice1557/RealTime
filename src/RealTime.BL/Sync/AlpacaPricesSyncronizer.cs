namespace RealTime.BL.Sync
{
    using Alpaca.Markets;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RealTime.BL.Common;
    using RealTime.DAL;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class AlpacaPricesSyncronizer : IPricesSyncronizer
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly IOptions<AppSettings> settings;

        public AlpacaPricesSyncronizer(
            IServiceProvider serviceProvider,
            ILogger<AlpacaPricesSyncronizer> logger,
            IOptions<AppSettings> settings)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.settings = settings;
        }

        public async Task SyncIntradayPrices(string[] symbols, CancellationToken cancellationToken)
        {
            List<FundToSync> fundsToSync;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                fundsToSync = await dbContext.Funds
                    .Where(x => symbols.Contains(x.Symbol))
                    .OrderByDescending(x => x.Volume)
                    .Select(x => new FundToSync { Id = x.FundId, Symbol = x.Symbol })
                    .ToListAsync(cancellationToken);
            }

            await SyncIntradayPrices(fundsToSync, cancellationToken);
        }

        public async Task SyncDailyPrices(string[] symbols, CancellationToken cancellationToken)
        {
            List<FundToSync> fundsToSync;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                fundsToSync = await dbContext.Funds
                    .Where(x => symbols.Contains(x.Symbol))
                    .OrderByDescending(x => x.Volume)
                    .Select(x => new FundToSync { Id = x.FundId, Symbol = x.Symbol })
                    .ToListAsync(cancellationToken);
            }

            await this.SyncDailyPrices(fundsToSync, cancellationToken);
        }

        public async Task SyncDailyPrices(CancellationToken cancellationToken)
        {
            List<FundToSync> fundsToSync;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                fundsToSync = await dbContext.Funds.OrderByDescending(x => x.Volume)
                    .Select(x => new FundToSync 
                    {
                        Id = x.FundId,
                        Symbol = x.Symbol,
                        AnyDailyPrice = x.DailyPrices.Any()
                    })
                    .ToListAsync(cancellationToken);
            }

            await this.SyncDailyPrices(fundsToSync, cancellationToken);
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

            await SyncIntradayPrices(fundsToSync, cancellationToken);
        }

        private async Task SyncIntradayPrices(List<FundToSync> fundsToSync, CancellationToken cancellationToken)
        {
            var firstTime = true;
            var loopTimespan = TimeSpan.FromMinutes(1);
            while (!cancellationToken.IsCancellationRequested)
            {
                var sw = new Stopwatch();
                sw.Start();

                var fundsChunks = fundsToSync.Chunks(200);
                foreach (var fundsChunk in fundsChunks)
                {
                    var symbols = fundsChunk.Select(x => x.Symbol).ToArray();
                    IReadOnlyDictionary<string, IEnumerable<IAgg>> barsResponse;
                    using (var client = new RestClient(
                        this.settings.Value.AlpacaApiKey,
                        this.settings.Value.AlpacaApiSecret,
                        "https://paper-api.alpaca.markets"))
                    {
                        barsResponse = await client.GetBarSetAsync(
                            symbols,
                            TimeFrame.Minute,
                            limit: firstTime ? 1000 : 20,
                            cancellationToken: cancellationToken);
                    }

                    foreach (var fund in fundsChunk)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        if (!barsResponse.ContainsKey(fund.Symbol))
                        {
                            this.logger.LogWarning($"Failed to load intraday prices for {fund.Symbol}");
                            continue;
                        }

                        var bars = barsResponse[fund.Symbol].ToArray();
                        if (bars.Length == 0)
                        {
                            this.logger.LogWarning($"Empty intraday prices for {fund.Symbol}");
                            continue;
                        }

                        try
                        {
                            this.logger.LogInformation($"Syncing intraday for {fund.Symbol}");
                            using (var scope = serviceProvider.CreateScope())
                            {
                                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                                await SyncIntradayFundPrices(
                                    dbContext,
                                    fund.Id,
                                    bars,
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
                }

                sw.Stop();
                if (sw.Elapsed < loopTimespan)
                {
                    var waitTimespan = loopTimespan - sw.Elapsed;
                    this.logger.LogInformation(
                        $"Sync loop finished in {sw.Elapsed.TotalSeconds:F02} s. Next loop in {waitTimespan.TotalSeconds:F02} s.");
                    await Task.Delay(waitTimespan, cancellationToken);
                }

                firstTime = false;
            }
        }

        private async Task SyncDailyPrices(List<FundToSync> fundsToSync, CancellationToken cancellationToken)
        {
            var fundsChunks = fundsToSync.Chunks(200);
            foreach (var fundsChunk in fundsChunks)
            {
                var symbols = fundsChunk.Select(x => x.Symbol).ToArray();
                IReadOnlyDictionary<string, IEnumerable<IAgg>> barsResponse;
                using (var client = new RestClient(
                    this.settings.Value.AlpacaApiKey,
                    this.settings.Value.AlpacaApiSecret,
                    "https://paper-api.alpaca.markets"))
                {
                    barsResponse = await client.GetBarSetAsync(
                        symbols,
                        TimeFrame.Day,
                        limit: fundsChunk.Any(x => !x.AnyDailyPrice) ? 200 : 20,
                        cancellationToken: cancellationToken);
                }

                foreach (var fund in fundsChunk)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!barsResponse.ContainsKey(fund.Symbol))
                    {
                        this.logger.LogWarning($"Failed to load daily prices for {fund.Symbol}");
                        continue;
                    }

                    var bars = barsResponse[fund.Symbol].ToArray();
                    if (bars.Length == 0)
                    {
                        this.logger.LogWarning($"Empty daily prices for {fund.Symbol}");
                        continue;
                    }

                    try
                    {
                        this.logger.LogInformation($"Syncing daily for {fund.Symbol}");
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                            await SyncDailyFundPrices(
                                dbContext,
                                fund.Id,
                                bars,
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
        }

        private async Task SyncIntradayFundPrices(
            PricesDbContext dbContext,
            int id,
            IAgg[] bars,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var minDate = bars.Min(b => b.Time);
            var maxDate = bars.Max(b => b.Time);

            var exisingPricesTimestamp = await dbContext.Prices
                .Where(
                    x => x.FundId == id
                            && x.Timestamp >= minDate
                            && x.Timestamp <= maxDate)
                .Select(x => x.Timestamp)
                .ToListAsync();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var priceBulkCount = 0;
            foreach (var bar in bars)
            {
                if (exisingPricesTimestamp.Contains(bar.Time))
                {
                    continue;
                }

                priceBulkCount++;
                dbContext.Prices.Add(
                    new DAL.Entities.Price
                    {
                        FundId = id,
                        Timestamp = bar.Time,
                        Close = bar.Close,
                        High = bar.High,
                        Open = bar.Open,
                        Low = bar.Low,
                        Volume = bar.Volume
                    });

                if (priceBulkCount >= 100)
                {
                    await dbContext.SaveChangesAsync();
                    priceBulkCount = 0;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task SyncDailyFundPrices(
            PricesDbContext dbContext,
            int id,
            IAgg[] bars,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var minDate = bars.Min(b => b.Time);
            var maxDate = bars.Max(b => b.Time);
            var exisingPricesTimestamp = await dbContext.DailyPrices
                .Where(
                    x => x.FundId == id
                         && x.Timestamp >= minDate
                         && x.Timestamp <= maxDate)
                .Select(x => x.Timestamp)
                .ToListAsync();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var priceBulkCount = 0;
            foreach (var bar in bars)
            {
                if (exisingPricesTimestamp.Contains(bar.Time))
                {
                    continue;
                }

                priceBulkCount++;
                dbContext.DailyPrices.Add(
                    new DAL.Entities.DailyPrice
                    {
                        FundId = id,
                        Timestamp = bar.Time,
                        Close = bar.Close,
                        High = bar.High,
                        Open = bar.Open,
                        Low = bar.Low,
                        Volume = bar.Volume
                    });

                if (priceBulkCount >= 100)
                {
                    await dbContext.SaveChangesAsync();
                    priceBulkCount = 0;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }

        private class FundToSync
        {
            public int Id { get; set; }

            public string Symbol { get; set; }

            public bool AnyDailyPrice { get; set; }
        }
    }
}
