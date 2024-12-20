namespace RealTime.BL.Sync
{
    using global::Alpaca.Markets;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RealTime.BL.Common;
    using RealTime.BL.Polygon;
    using RealTime.BL.Prices;
    using RealTime.DAL;

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class AlpacaPricesSyncronizer : IPricesSyncronizer, IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly IOptions<AppSettings> settings;
        private readonly Lazy<IAlpacaDataClient> alpacaClientLazy;
        private readonly Lazy<IAlpacaTradingClient> alpacaTradingClientLazy;

        private readonly BlockingCollection<FundWithPrice> pricesQueue = new BlockingCollection<FundWithPrice>();
        private readonly ICustomPolygonStreamingClient customPolygonStreamingClient;
        private readonly CalendarService calendarService;

        public AlpacaPricesSyncronizer(
            IServiceProvider serviceProvider,
            ILogger<AlpacaPricesSyncronizer> logger,
            IOptions<AppSettings> settings,
            ICustomPolygonStreamingClient customPolygonStreamingClient,
            CalendarService calendarService)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.settings = settings;
            this.alpacaClientLazy = new Lazy<IAlpacaDataClient>(CreateAlpacaClient);
            this.alpacaTradingClientLazy = new Lazy<IAlpacaTradingClient>(CreateTradingAlpacaClient);
            this.customPolygonStreamingClient = customPolygonStreamingClient;
            this.calendarService = calendarService;
        }

        public async Task SyncIntradayPrices(string[] symbols, CancellationToken cancellationToken)
        {
            try
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
            catch (TaskCanceledException)
            {
            }
        }

        public async Task SyncDailyPrices(string[] symbols, CancellationToken cancellationToken)
        {
            try
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
            catch (TaskCanceledException)
            {
            }
        }

        public async Task SyncDailyPrices(CancellationToken cancellationToken)
        {
            try
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
            catch (TaskCanceledException)
            {
            }
        }

        public async Task SyncIntradayPrices(CancellationToken cancellationToken)
        {
            try
            {
                List<FundToSync> fundsToSync;
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                    fundsToSync = await dbContext.Funds.OrderByDescending(x => x.Volume)
                        .Select(x => new FundToSync { Id = x.FundId, Symbol = x.Symbol })
                        .ToListAsync(cancellationToken);
                }

                await SyncIntradayPrices(fundsToSync, cancellationToken);
            }
            catch (TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            if (alpacaClientLazy.IsValueCreated)
            {
                alpacaClientLazy.Value.Dispose();
            }
        }

        private async Task SyncIntradayPrices(List<FundToSync> fundsToSync, CancellationToken cancellationToken)
        {
            if (fundsToSync.Count == 0)
            {
                throw new Exception("Empty symbols list to sync");
            }

            try
            {
                await SyncFirstIntradayPrices(fundsToSync, cancellationToken);
            }
            catch(Exception e)
            {
                this.logger.LogError(e, "Failed to sync first prices");
            }

            this.logger.LogInformation("Subscribing on prices events");
            customPolygonStreamingClient.SecondAggReceived += agg => AddToQueue(fundsToSync, agg, false);
            customPolygonStreamingClient.MinuteAggReceived += agg => AddToQueue(fundsToSync, agg, true);

            var symbolsToSync = fundsToSync.Select(x => x.Symbol).ToList();
            customPolygonStreamingClient.OnError +=
                async (e) =>
                {
                    this.logger.LogError(e, "Polygon error.");
                    var reconnect = customPolygonStreamingClient.IsDisconnected;
                    if (reconnect)
                    {
                        await ConnectWithDelay(symbolsToSync, cancellationToken);
                    }
                };

            await ConnectWithDelay(symbolsToSync, cancellationToken);

            await ProcessQueue(cancellationToken);
        }

        private async Task ConnectWithDelay(List<string> symbolsToSync, CancellationToken cancellationToken)
        {
            var connectionAttempts = 0;
            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    var delay = TimeSpan.Zero;
                    if (connectionAttempts == 1)
                    {
                        delay = TimeSpan.FromSeconds(10);
                    }
                    else if (connectionAttempts > 1)
                    {
                        delay = TimeSpan.FromSeconds(60);
                    }

                    if (delay != TimeSpan.Zero)
                    {
                        this.logger.LogInformation($"Waiting for {delay:c} to reconnect.");
                        await Task.Delay(delay, cancellationToken);
                    }

                    await ConnectAndSubscribe(symbolsToSync, cancellationToken);
                    break;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error when try to connect to polygon.");
                    connectionAttempts++;
                }

            } while (true);
        }

        private async Task<DateTime> WaitForMarketOpens(CancellationToken cancellationToken)
        {
            IClock clock;
            try
            {
                clock = await alpacaTradingClientLazy.Value.GetClockAsync();
            }
            catch(Exception e)
            {
                logger.LogError(e, "Failed to read calendar from Alpaca. Fallback to local saved copy");
                clock = calendarService.GetClock();
            }

            if (!clock.IsOpen)
            {
                var timeUntilMarketOpens = clock.NextOpenUtc - DateTime.UtcNow;
                if (timeUntilMarketOpens > TimeSpan.FromHours(1))
                {
                    timeUntilMarketOpens = TimeSpan.FromHours(1);
                }

                this.logger.LogWarning($"Market is closed. Waiting for {timeUntilMarketOpens:c} to reconnect.");
                await Task.Delay(timeUntilMarketOpens, cancellationToken);

                return await WaitForMarketOpens(cancellationToken);
            }

            return clock.NextCloseUtc;
        }

        private CancellationTokenSource _reconnectTaskCancelationTokenSource;
        private Task _reconnectTask;

        private async Task ConnectAndSubscribe(List<string> symbolsToSync, CancellationToken cancellationToken)
        {
            var nextCloseDateUtc = await WaitForMarketOpens(cancellationToken);
            var timeUntilMarketCloses = nextCloseDateUtc - DateTime.UtcNow;

            this.logger.LogWarning("Trying to connect to polygon");
            this.logger.LogInformation($"Connecting to Polygon. Symbols count: {symbolsToSync.Count}");
            await customPolygonStreamingClient.Connect(cancellationToken);

            customPolygonStreamingClient.SubscribeMinuteAgg(symbolsToSync);
            // customPolygonStreamingClient.SubscribeSecondAgg(symbolsToSync);
            this.logger.LogWarning("Succesfully connected to Polygon");

            if (_reconnectTask != null && !_reconnectTask.IsCompleted)
            {
                _reconnectTaskCancelationTokenSource.Cancel();
                _reconnectTask = null;
            }

            _reconnectTaskCancelationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _reconnectTask = Task.Factory.StartNew(
                async () =>
                {
                    this.logger.LogInformation($"Wating for {timeUntilMarketCloses:c} until market closes to disconnect");
                    await Task.Delay(timeUntilMarketCloses, _reconnectTaskCancelationTokenSource.Token);
                    this.logger.LogInformation("Disconnecting until market opens");
                    if (!customPolygonStreamingClient.IsDisconnected)
                    {
                        this.logger.LogInformation("Starting disconnecting until market opens");
                        customPolygonStreamingClient.UnSubscribeMinuteAgg(symbolsToSync);
                        // customPolygonStreamingClient.UnSubscribeSecondAgg(symbolsToSync);
                        await customPolygonStreamingClient.DisconnectAsync();

                        _ = Task.Run(() => ConnectWithDelay(symbolsToSync, cancellationToken));
                    }
                },
                _reconnectTaskCancelationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void AddToQueue(List<FundToSync> fundsToSync, AggregatedPrice agg, bool isMinute)
        {
            var period = isMinute ? "minute" : "second";
            this.logger.LogInformation($"Recieved {period} price: {agg.StartTime} {agg.Symbol} ${agg.Close} ");

            var fund = fundsToSync.FirstOrDefault(x => x.Symbol == agg.Symbol);
            if (fund == null)
            {
                this.logger.LogWarning($"Recieved {period} price of {agg.Symbol}, but not subscribing on this ticker");
                return;
            }

            pricesQueue.Add(
                new FundWithPrice(
                    fund.Id,
                    fund.Symbol,
                    agg.StartTime,
                    agg.Open,
                    agg.Close,
                    agg.High,
                    agg.Low,
                    agg.Volume,
                    isMinute));
        }

        private async Task ProcessQueue(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var fundWithPrice = this.pricesQueue.Take(cancellationToken);
                    try
                    {
                        this.logger.LogInformation($"Message for {fundWithPrice.Symbol} on {fundWithPrice.Timestamp}");
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var timeStamp = fundWithPrice.IsMinute ?
                                fundWithPrice.Timestamp : CeilingMinute(fundWithPrice.Timestamp);

                            await using (var dbContext = scope.ServiceProvider.GetService<PricesDbContext>())
                            {
                                var price = await dbContext.Prices
                                    .SingleOrDefaultAsync(
                                    p => p.FundId == fundWithPrice.Id && p.Timestamp == timeStamp,
                                    cancellationToken);

                                if (price == null)
                                {
                                    price = new DAL.Entities.Price
                                    {
                                        FundId = fundWithPrice.Id,
                                        Timestamp = timeStamp,
                                        Close = fundWithPrice.Close,
                                        High = fundWithPrice.High,
                                        Open = fundWithPrice.Open,
                                        Low = fundWithPrice.Low,
                                        Volume = fundWithPrice.Volume
                                    };

                                    dbContext.Prices.Add(price);
                                }
                                else
                                {
                                    if (fundWithPrice.IsMinute)
                                    {
                                        price.Close = fundWithPrice.Close;
                                        price.High = fundWithPrice.High;
                                        price.Open = fundWithPrice.Open;
                                        price.Low = fundWithPrice.Low;
                                        price.Volume = fundWithPrice.Volume;
                                    }
                                    else
                                    {
                                        price.Close = fundWithPrice.Close;
                                        price.High = price.High > fundWithPrice.High ? price.High : fundWithPrice.High;
                                        price.Low = price.Low < fundWithPrice.Low ? price.Low : fundWithPrice.Low;
                                        price.Volume = fundWithPrice.Volume;
                                    }
                                }

                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.LogError(e, $"Failed to sync intraday prices for {fundWithPrice.Symbol}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task SyncFirstIntradayPrices(List<FundToSync> fundsToSync, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("First time intraday sync");

            var calendar = await alpacaTradingClientLazy.Value.ListCalendarAsync(
                new CalendarRequest().SetInclusiveTimeInterval(
                    DateTime.UtcNow.AddDays(-100),
                    DateTime.UtcNow.AddDays(1)),
                cancellationToken);
            var calendarDict = calendar.ToDictionary(x => x.TradingDateUtc);

            var today = DateTime.UtcNow;
            var currentTradingDayOpenUtc = calendar
                .Where(x => x.TradingOpenTimeUtc <= today)
                .OrderByDescending(x => x.TradingOpenTimeUtc)
                .First()
                .TradingOpenTimeUtc;

            List<FundToSync> fundsToInitialSync;
            using (var scope = serviceProvider.CreateScope())
            {
                await using (var dbContext = scope.ServiceProvider.GetService<PricesDbContext>())
                {
                    var fundIdsWithCurrentDayPrices = await dbContext.Prices
                        .Where(x => x.Timestamp >= currentTradingDayOpenUtc)
                        .Select(x => x.FundId)
                        .Distinct()
                        .ToListAsync();
                    var fundIdsHastSet = fundIdsWithCurrentDayPrices.ToHashSet();

                    fundsToInitialSync = fundsToSync
                        .Where(x => !fundIdsHastSet.Contains(x.Id))
                        .ToList();
                }
            }
            foreach (var fundsChunk in fundsToInitialSync.Chunks(20))
            {
                var symbols = fundsChunk.Select(x => x.Symbol).ToArray();
                var request = new HistoricalBarsRequest(symbols, DateTime.UtcNow.AddHours(-36), DateTime.UtcNow, BarTimeFrame.Minute);
                request.Pagination.Size = 10000;
                var result = new Dictionary<string, List<IBar>>();
                var haveNextPage = false;
                do
                {
                    var barsResponse = await alpacaClientLazy.Value.GetHistoricalBarsAsync(request, cancellationToken);
                    foreach(var (key, items) in barsResponse.Items)
                    {
                        if (result.ContainsKey(key))
                        {
                            result[key].AddRange(items);
                        }
                        else
                        {
                            result.Add(key, new List<IBar>(items));
                        }
                    }

                    haveNextPage = !string.IsNullOrWhiteSpace(barsResponse.NextPageToken);
                    request.Pagination.Token = barsResponse.NextPageToken;
                }
                while (haveNextPage);

                foreach (var fund in fundsChunk)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!result.ContainsKey(fund.Symbol))
                    {
                        this.logger.LogWarning($"Failed to load intraday prices for {fund.Symbol}");
                        continue;
                    }

                    var bars = result[fund.Symbol]
                        .Where(x => calendarDict.ContainsKey(x.TimeUtc.Date))
                        .Select(x => new
                        {
                            Item = x,
                            Time = x.TimeUtc,
                            MarketDay = calendarDict[x.TimeUtc.Date]
                        })
                       .Where(x => x.Time >= x.MarketDay.TradingOpenTimeUtc
                                    && x.Time <= x.MarketDay.TradingCloseTimeUtc)
                       .Select(x => x.Item)
                       .ToArray();

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
                            await using (var dbContext = scope.ServiceProvider.GetService<PricesDbContext>())
                            {
                                await SyncIntradayFundPrices(
                                    dbContext,
                                    fund.Id,
                                    bars,
                                    cancellationToken);
                            }
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
        }

        private async Task SyncDailyPrices(List<FundToSync> fundsToSync, CancellationToken cancellationToken)
        {
            var fundsChunks = fundsToSync.Chunks(50);
            foreach (var fundsChunk in fundsChunks)
            {
                var symbols = fundsChunk.Select(x => x.Symbol).ToArray();
                var daysCount = fundsChunk.Any(x => !x.AnyDailyPrice) ? 1000u : 20u;
                var request = new HistoricalBarsRequest(symbols, DateTime.UtcNow.AddDays(-daysCount), DateTime.UtcNow, BarTimeFrame.Minute);
                var barsResponse = await alpacaClientLazy.Value.GetHistoricalBarsAsync(request, cancellationToken);
                foreach (var fund in fundsChunk)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!barsResponse.Items.ContainsKey(fund.Symbol))
                    {
                        this.logger.LogWarning($"Failed to load daily prices for {fund.Symbol}");
                        continue;
                    }

                    var bars = barsResponse.Items[fund.Symbol].ToArray();
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
            IBar[] bars,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var minDate = bars.Min(b => b.TimeUtc);
            var maxDate = bars.Max(b => b.TimeUtc);

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
                if (exisingPricesTimestamp.Contains(bar.TimeUtc))
                {
                    continue;
                }

                priceBulkCount++;
                dbContext.Prices.Add(
                    new DAL.Entities.Price
                    {
                        FundId = id,
                        Timestamp = bar.TimeUtc,
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
            IBar[] bars,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var minDate = bars.Min(b => b.TimeUtc).Date;
            var maxDate = bars.Max(b => b.TimeUtc).Date;
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
                var date = bar.TimeUtc.Date;
                if (exisingPricesTimestamp.Contains(date))
                {
                    continue;
                }

                priceBulkCount++;
                dbContext.DailyPrices.Add(
                    new DAL.Entities.DailyPrice
                    {
                        FundId = id,
                        Timestamp = date,
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

        private IAlpacaDataClient CreateAlpacaClient()
        {
            if (string.IsNullOrEmpty(this.settings.Value.AlpacaApiKey) || string.IsNullOrEmpty(this.settings.Value.AlpacaApiSecret))
            {
                throw new InvalidOperationException("Alpaca settings is not present");
            }

            var secretKey = new SecretKey(this.settings.Value.AlpacaApiKey, this.settings.Value.AlpacaApiSecret);

            return Environments.Live.GetAlpacaDataClient(secretKey);
        }

        private IAlpacaTradingClient CreateTradingAlpacaClient()
        {
            if (string.IsNullOrEmpty(this.settings.Value.AlpacaApiKey) || string.IsNullOrEmpty(this.settings.Value.AlpacaApiSecret))
            {
                throw new InvalidOperationException("Alpaca settings is not present");
            }

            var secretKey = new SecretKey(this.settings.Value.AlpacaApiKey, this.settings.Value.AlpacaApiSecret);

            return Environments.Live.GetAlpacaTradingClient(secretKey);
        }

        private static DateTime CeilingMinute(DateTime dateTime)
        {
            return new DateTime(
                dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerMinute),
                dateTime.Kind
                )
                .AddMinutes(1);
        }

        private class FundToSync
        {
            public int Id { get; set; }

            public string Symbol { get; set; }

            public bool AnyDailyPrice { get; set; }
        }

        private struct FundWithPrice
        {
            public FundWithPrice(
                int id,
                string symbol,
                DateTime timestamp,
                decimal open,
                decimal close,
                decimal high,
                decimal low,
                long volume,
                bool isMinute)
            {
                Id = id;
                Symbol = symbol;
                Timestamp = timestamp;
                Open = open;
                Close = close;
                High = high;
                Low = low;
                Volume = volume;
                IsMinute = isMinute;
            }

            public int Id { get; }

            public string Symbol { get; }

            public DateTime Timestamp { get; }

            public decimal Open { get; }

            public decimal Close { get; }

            public decimal High { get; }

            public decimal Low { get; }

            public long Volume { get; }

            public bool IsMinute { get; }
        }
    }
}
