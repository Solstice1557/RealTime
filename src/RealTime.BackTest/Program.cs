using RealTime.BL.Prices;
using RealTime.BL.Trading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RealTime.BackTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start backtest");
            var settings = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText("config.json"));
            if (string.IsNullOrEmpty(settings.DataPath))
            {
                Console.WriteLine("No data path!");
                return;
            }

            if (settings.Tickers == null || settings.Tickers.Length == 0)
            {
                Console.WriteLine("Empty tickers list!");
                return;
            }

            var pricesDict = settings.Tickers
                .Select(ticker =>
                    {
                        var filePath = GetPricesFile(settings.DataPath, ticker);
                        if (string.IsNullOrEmpty(filePath))
                        {
                            Console.WriteLine("Prices file not found for " + ticker);
                        }

                        return new { ticker, filePath };
                    })
                .Where(x => !string.IsNullOrEmpty(x.filePath))
                .ToDictionary(x => x.ticker, x => PricesService.GetPrices(x.filePath, settings.MinPriceDate));
            if (pricesDict.Count == 0)
            {
                Console.WriteLine("Files not found!");
                return;
            }

            var tradingHistories = pricesDict.Keys
                .ToDictionary(ticker => ticker, _ => new TradingHistory());

            IEnumerable<BacktestPriceItem> backtestPrices = GetBacktestPrices(pricesDict);

            var lastPrices = BacktestService.Test(tradingHistories, backtestPrices);
            decimal totalProfit = 0;
            foreach (var th in tradingHistories)
            {
                if (!lastPrices.ContainsKey(th.Key))
                {
                    Console.WriteLine($"Could not calculate profit for ${th.Key} - no price");
                    continue;
                }

                var profit = th.Value.GetCurrentProfit(lastPrices[th.Key]);
                totalProfit += profit;
                Console.WriteLine($"Current trading profit for ${th.Key}: ${profit:F02}");
            }

            Console.WriteLine($"Total trading profit: ${totalProfit:F02}");
            Console.WriteLine("Finish backtest");
        }

        private static IEnumerable<BacktestPriceItem> GetBacktestPrices(
            Dictionary<string, IEnumerable<(PriceModel oneMinPrice, PriceModel fiveMinPrice)>> pricesDict)
        {
            var currentPrices = new Dictionary<string, (PriceModel oneMinPrice, PriceModel fiveMinPrice)>();
            var tickers = pricesDict.Keys.ToList();
            var enumerators = pricesDict.ToDictionary(x => x.Key, x => x.Value.GetEnumerator());
            foreach (var pr in enumerators)
            {
                if (pr.Value.MoveNext())
                {
                    currentPrices[pr.Key] = pr.Value.Current;
                }
            }

            while (currentPrices.Count != 0)
            {
                var minDate = currentPrices.Select(x => x.Value.oneMinPrice.Date).Min();
                var backtestItem = new BacktestPriceItem(minDate);
                foreach (var ticker in tickers)
                {
                    if (!currentPrices.ContainsKey(ticker))
                    {
                        backtestItem.OneMinPrices[ticker] = null;
                        backtestItem.FiveMinPrices[ticker] = null;
                        continue;
                    }

                    var currentPrice = currentPrices[ticker];
                    var currentPriceDate = currentPrice.oneMinPrice.Date;
                    var enumerator = enumerators[ticker];
                    if (currentPriceDate == minDate)
                    {
                        backtestItem.OneMinPrices[ticker] = currentPrice.oneMinPrice;
                        backtestItem.FiveMinPrices[ticker] = currentPrice.fiveMinPrice;
                        if (enumerator.MoveNext())
                        {
                            currentPrices[ticker] = enumerator.Current;
                        }
                        else
                        {
                            currentPrices.Remove(ticker);
                        }
                    }
                    else
                    {
                        backtestItem.OneMinPrices[ticker] = null;
                        backtestItem.FiveMinPrices[ticker] = null;
                    }
                }

                yield return backtestItem;
            }

            foreach (var e in enumerators)
            {
                e.Value.Dispose();
            }
        }

        private static string GetPricesFile(string dataPath, string ticker)
        {
            var files = Directory.GetFiles(dataPath)
                .Where(filePath => Path.GetFileName(filePath).StartsWith(ticker))
                .ToArray();
            return files.FirstOrDefault(x => x.Contains("1min"))
                ?? files.FirstOrDefault(x => x.Contains("5min"))
                ?? files.FirstOrDefault(x => x.Contains("30min"))
                ?? files.FirstOrDefault(x => x.Contains("1hour"))
                ?? files.FirstOrDefault(x => x.Contains("1day"))
                ?? files.FirstOrDefault();
        }
    }
}
