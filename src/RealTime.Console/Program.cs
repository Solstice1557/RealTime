namespace RealTime.Console
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using RealTime.DAL;
    using RealTime.BL.Prices;
    using RealTime.BL.Sync;
    using RealTime.BL.Trading;

    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = Initialization.InitServices();
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                dbContext.Database.Migrate();
            }

            var cancellationTokenSource = new CancellationTokenSource();
            //var syncTask = Task.Run(async () => await Sync(serviceProvider, cancellationTokenSource.Token));

            Console.WriteLine("Sync in progress, press any key to start calculation");
            Console.ReadKey();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    await Calculations(scope.ServiceProvider);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            Console.WriteLine("End. Press any key to stop prices syncronization");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            //syncTask.Wait(TimeSpan.FromSeconds(20));
        }

        private static async Task Sync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            var syncronizer = serviceProvider.GetService<IPricesSyncronizer>();
            Console.WriteLine("Sync daily prices");
            await syncronizer.SyncDailyPrices(cancellationToken);
            Console.WriteLine("Sync intraday prices");
            await syncronizer.SyncIntradayPrices(cancellationToken);
        }

        private static async Task Calculations(IServiceProvider serviceProvider)
        {
            var pricesService = serviceProvider.GetService<IPricesService>();

            const string Symbol = "AAPL";
            const PricesTimeInterval Interval = PricesTimeInterval.Intraday1Min;

            var techAnalyses = new[]
                {
                    new TechAnalysisInfo(TechAnalysisType.SmoothedMovingAverage, 20, PriceType.Close),
                    new TechAnalysisInfo(TechAnalysisType.ExponentalMovingAverage, 20, PriceType.Close),
                    // new TechAnalysisInfo(TechAnalysisType.ExponentalMovingAverage, 150, PriceType.Close),
                    new TechAnalysisInfo(TechAnalysisType.MovingAverage, 40, PriceType.Low)
                };

            var prices = await pricesService.GetPrices(Symbol, Interval, 100, null, null, techAnalyses);

            Console.WriteLine($"Prices for {Symbol} {Interval}");
            Console.WriteLine();

            foreach (var price in prices)
            {
                Console.WriteLine(
                    "{0:g}: 1- {1:F2}, 2- {2:F2}, 3- {3:F2}, 4- {4:F2}{5}",
                    price.Date,
                    price.Open,
                    price.Close,
                    price.High,
                    price.Low,
                    GetTaString(price.TechAnalysis));
            }

            var tradingHistory = new TradingHistory();
            tradingHistory.Buy(prices[10].Date, prices[10].Close.Value, 100);
            tradingHistory.Buy(prices[20].Date, prices[20].Close.Value, 50);
            tradingHistory.Sell(prices[30].Date, prices[30].Close.Value, 140);

            var profit = tradingHistory.GetCurrentProfit(prices.Last().Close.Value);
            Console.WriteLine($"Current trading profit: ${profit:F02}");

            HtmlConverter.SavePricesHtml(prices, tradingHistory, Symbol, Interval.ToString(), "test.html");
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
    }
}
