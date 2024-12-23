﻿namespace RealTime.Console
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using RealTime.BL.Prices;
    using RealTime.BL.Sync;
    using RealTime.BL.Trading;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program
    {
        public async static Task Main()
        {
            await using var serviceProvider = Initialization.InitServices();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => logger.LogCritical("Unhandled exception", (Exception)e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (s, e) => logger.LogError("Unhandled task exception", e.Exception);

            var additionalStocksService = serviceProvider.GetRequiredService<AdditionalStocksImportService>();
            await additionalStocksService.ImportAdditionalStocks();

            var cancellationTokenSource = new CancellationTokenSource();
            var syncTask = Task.Run(async () =>
            {
                try
                {
                    await Sync(serviceProvider, cancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "Syncronization task error");
                }
            });

            Console.WriteLine("Sync in progress, press any key to start calculation");
            Console.ReadKey();

            Task tradingTask = Task.Run(
                    async () =>
                    {
                        try
                        {
                            using (var scope = serviceProvider.CreateScope())
                            {
                                var pricesService = serviceProvider.GetService<IPricesService>();
                                await Calculations(pricesService, cancellationTokenSource.Token);
                            }
                        }
                        catch (TaskCanceledException)
                        {
                        }
                        catch (Exception e)
                        {
                            logger.LogCritical(e, "Trading task error");
                        }
                    });

            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            syncTask.Wait(TimeSpan.FromSeconds(20));
            tradingTask.Wait(TimeSpan.FromSeconds(20));
        }

        private static async Task Sync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var syncronizer = serviceProvider.GetService<IPricesSyncronizer>();
            Console.WriteLine("Sync daily prices");
            await syncronizer.SyncDailyPrices(cancellationToken);
            // funds to sync can be set up explicitly:
            // await syncronizer.SyncDailyPrices(new[] { "AAPL", "MSFT" }, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Console.WriteLine("Sync intraday prices");
            await syncronizer.SyncIntradayPrices(cancellationToken);
            // funds to sync can be set up explicitly:
            // await syncronizer.SyncIntradayPrices(new[] { "AAPL", "MSFT" }, cancellationToken);
        }

        private static async Task Calculations(IPricesService pricesService, CancellationToken cancellationToken)
        {
            const string Symbol = "AAPL";

            var techAnalyses = new[]
                {
                    new TechAnalysisInfo(TechAnalysisType.SmoothedMovingAverage, 20, PriceType.Close),
                    new TechAnalysisInfo(TechAnalysisType.ExponentalMovingAverage, 20, PriceType.Close),
                    new TechAnalysisInfo(TechAnalysisType.MovingAverage, 40, PriceType.Low)
                };

            var intervals = new[]
                {
                    PricesTimeInterval.Intraday1Min,
                    PricesTimeInterval.Intraday5Min,
                    PricesTimeInterval.Intraday15Min
                };

            var tradingHistory = new TradingHistory();

            List<Dictionary<PricesTimeInterval, PriceModel>> prices = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                prices = await pricesService.GetPrices(
                                    Symbol,
                                    intervals,
                                    100,
                                    null,
                                    null,
                                    techAnalyses);
                if (prices.Count == 0)
                {
                    Console.WriteLine($"No prices for {Symbol}");
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                    continue;
                }

                Console.WriteLine($"Prices for {Symbol}");
                Console.WriteLine();

                foreach (var price in prices)
                {
                    foreach (var interval in intervals)
                    {
                        // Console.WriteLine(price[interval].ToDebugString(interval));
                    }
                }

                var lastPrice = prices.First()[intervals[0]];
                Console.WriteLine(
                    "{2:HH:mm:ss} Last Price {0:dd.MM HH:mm} ${1}",
                    lastPrice.Date,
                    lastPrice.Close.Value,
                    DateTime.Now);

                // todo your code here
                tradingHistory.Buy(lastPrice.Date, lastPrice.Close.Value, 100);

                // calculations and displaying current profit
                var profit = tradingHistory.GetCurrentProfit(prices.First()[intervals[0]].Close.Value);
                Console.WriteLine($"Current trading profit: ${profit:F02}");

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            if (prices != null)
            {
                var profit = tradingHistory.GetCurrentProfit(prices.First()[intervals[0]].Close.Value);
                Console.WriteLine($"Current trading profit: ${profit:F02}");

                HtmlConverter.SavePricesHtml(
                    prices,
                    tradingHistory,
                    Symbol,
                    intervals,
                    "test.html");
            }
        }
    }
}
