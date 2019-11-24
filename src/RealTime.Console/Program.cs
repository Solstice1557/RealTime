namespace RealTime.Console
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using RealTime.BL.Prices;
    using RealTime.BL.Sync;
    using RealTime.BL.Trading;

    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = Initialization.InitServices();
            var cancellationTokenSource = new CancellationTokenSource();
            var syncTask = Task.Run(async () => await Sync(serviceProvider, cancellationTokenSource.Token));

            Console.WriteLine("Sync in progress, press any key to start calculation");
            Console.ReadKey();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    await Calculations(scope.ServiceProvider);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("End. Press any key to stop prices syncronization");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            syncTask.Wait(TimeSpan.FromSeconds(20));
        }

        private static async Task Sync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            try
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
            catch (TaskCanceledException)
            {
            }
        }

        private static async Task Calculations(IServiceProvider serviceProvider)
        {
            var pricesService = serviceProvider.GetService<IPricesService>();

            const string Symbol = "AAPL";

            var techAnalyses = new[]
                {
                    new TechAnalysisInfo(TechAnalysisType.SmoothedMovingAverage, 20, PriceType.Close),
                    new TechAnalysisInfo(TechAnalysisType.ExponentalMovingAverage, 20, PriceType.Close),
                    // new TechAnalysisInfo(TechAnalysisType.ExponentalMovingAverage, 150, PriceType.Close),
                    new TechAnalysisInfo(TechAnalysisType.MovingAverage, 40, PriceType.Low)
                };

            var intervals = new[]
                {
                    PricesTimeInterval.Intraday1Min,
                    PricesTimeInterval.Intraday5Min,
                    PricesTimeInterval.Intraday15Min
                };

            var prices = await pricesService.GetPrices(
                Symbol,
                intervals,
                100,
                null,
                null,
                techAnalyses);

            Console.WriteLine($"Prices for {Symbol}");
            Console.WriteLine();

            foreach (var price in prices)
            {
                foreach (var interval in intervals)
                {
                    Console.WriteLine(
                        "{0:g} - {1}:  1- {2:F2}, 2- {3:F2}, 3- {4:F2}, 4- {5:F2}{6}",
                        price[interval].Date,
                        GetIntervalString(interval),
                        price[interval].Open,
                        price[interval].Close,
                        price[interval].High,
                        price[interval].Low,
                        GetTaString(price[interval].TechAnalysis));
                }

                Console.WriteLine();
            }

            var tradingHistory = new TradingHistory();
            tradingHistory.Buy(prices[10][intervals[0]].Date, prices[10][intervals[0]].Close.Value, 100);
            tradingHistory.Buy(prices[20][intervals[0]].Date, prices[20][intervals[0]].Close.Value, 50);
            tradingHistory.Sell(prices[30][intervals[0]].Date, prices[30][intervals[0]].Close.Value, 140);

            var profit = tradingHistory.GetCurrentProfit(prices.Last()[intervals[0]].Close.Value);
            Console.WriteLine($"Current trading profit: ${profit:F02}");

            HtmlConverter.SavePricesHtml(
                prices,
                tradingHistory,
                Symbol,
                intervals,
                "test.html");
        }

        private static string GetTaString(Dictionary<string, decimal?> dict)
        {
            var str = string.Empty;
            foreach (var kp in dict)
            {
                str += $", {kp.Key}: {kp.Value:F02}";
            }

            return str;
        }

        private static string GetIntervalString(PricesTimeInterval interval)
        {
            switch (interval)
            {
                case PricesTimeInterval.Intraday1Min:
                    return "1m";
                case PricesTimeInterval.Intraday5Min:
                    return "5m";
                case PricesTimeInterval.Intraday15Min:
                    return "15m";
                case PricesTimeInterval.Intraday30Min:
                    return "30m";
                case PricesTimeInterval.Intraday1Hour:
                    return "1h";
                case PricesTimeInterval.Daily:
                    return "day";
                case PricesTimeInterval.Weekly:
                    return "week";
                case PricesTimeInterval.Monthly:
                    return "Month";
            }

            return "unknown";
        }
    }
}
