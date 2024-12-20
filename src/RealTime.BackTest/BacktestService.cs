using RealTime.BL.Trading;
using System;
using System.Collections.Generic;

namespace RealTime.BackTest
{
    public static class BacktestService
    {
        public static Dictionary<string, decimal> Test(
            Dictionary<string, TradingHistory> histories,
            IEnumerable<BacktestPriceItem> prices)
        {
            var lastPrices = new Dictionary<string, decimal>();
            var buyAAPL = true;
            var buyBA = true;
            foreach (var price in prices)
            {
                UpdateLastPrices(lastPrices, price);

                // get prices logic
                var oneMinPriceAAPL = price.OneMinPrices["AAPL"];
                var fiveMinPriceBA = price.FiveMinPrices["BA"];
                var tradingHistoryAAPL = histories["AAPL"];
                var tradingHistoryBA = histories["BA"];

                // trading logic
                if (oneMinPriceAAPL != null)
                {
                    // price for AAPL exits
                    if (buyAAPL)
                    {
                        tradingHistoryAAPL.Buy(price.Date, oneMinPriceAAPL.Close ?? 0, 1);
                    }
                    else
                    {

                        tradingHistoryAAPL.Sell(price.Date, oneMinPriceAAPL.Close ?? 0, 1);
                    }

                    buyAAPL = !buyAAPL;
                }
                else
                {
                    Console.WriteLine($"No prices for AAPL on {price.Date}");
                }

                if (fiveMinPriceBA != null)
                {
                    // price for BA exits
                    if (buyBA)
                    {
                        tradingHistoryBA.Buy(price.Date, fiveMinPriceBA.Close ?? 0, 1);
                    }
                    else
                    {

                        tradingHistoryBA.Sell(price.Date, fiveMinPriceBA.Close ?? 0, 1);
                    }

                    buyBA = !buyBA;
                }
                else
                {
                    Console.WriteLine($"No prices for BA on {price.Date}");
                }
            }

            return lastPrices;
        }

        private static Dictionary<string, decimal> UpdateLastPrices(
            Dictionary<string, decimal> lastPrices,
            BacktestPriceItem price)
        {
            foreach (var (key, pr) in price.OneMinPrices)
            {
                if (pr != null)
                {
                    lastPrices[key] = pr.Close ?? pr.Open ?? 0;
                }
            }

            return lastPrices;
        }
    }
}
