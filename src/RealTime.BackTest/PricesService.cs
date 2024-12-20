using RealTime.BL.Prices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealTime.BackTest
{
    public static class PricesService
    {
        private static readonly int[] Periods = new[] { 3, 20, 70, 150 };

        private static Dictionary<int, decimal> Alphas =
            Periods.ToDictionary(x => x, timePeriod => 1.0m / timePeriod);

        private static Dictionary<int, decimal> Weights =
            Periods.ToDictionary(x => x, timePeriod => 1 - Alphas[timePeriod]);

        public static IEnumerable<(PriceModel oneMinPrice, PriceModel fiveMinPrice)> GetPrices(
            string fileName,
            DateTime minPriceDate)
        {
            var prices = PriceReader.Read(fileName);
            if (prices.Count == 0)
            {
                yield break;
            }

            var techAnalysis = new TechAnalysisService();
            var oneMinutePrevValues = Periods.ToDictionary(x => x, x => 0.0m);
            var fiveMinutesPrevValues = Periods.ToDictionary(x => x, x => 0.0m);
            PriceModel prevFiveMinutePrice = prices[0];

            for (int i = 0; i < prices.Count; i++)
            {
                var oneMinutePrice = prices[i];
                var price = oneMinutePrice.Close ?? oneMinutePrice.Open ?? 0;
                oneMinutePrevValues = GetPrevValues(oneMinutePrevValues, i, price);
                if (i % 5 == 0)
                {
                    prevFiveMinutePrice = oneMinutePrice.Clone();
                    fiveMinutesPrevValues = GetPrevValues(fiveMinutesPrevValues, i, price);
                }
                else
                {
                    prevFiveMinutePrice.Close = oneMinutePrice.Close;
                    prevFiveMinutePrice.High =
                        prevFiveMinutePrice.High.HasValue && oneMinutePrice.High.HasValue
                        ? Math.Max(prevFiveMinutePrice.High.Value, oneMinutePrice.High.Value)
                        : prevFiveMinutePrice.High ?? oneMinutePrice.High;
                    prevFiveMinutePrice.Low =
                        prevFiveMinutePrice.Low.HasValue && oneMinutePrice.Low.HasValue
                        ? Math.Min(prevFiveMinutePrice.Low.Value, oneMinutePrice.Low.Value)
                        : prevFiveMinutePrice.Low ?? oneMinutePrice.Low;
                }

                if (oneMinutePrice.Date < minPriceDate)
                {
                    continue;
                }

                AssignTechAnalysis(oneMinutePrevValues, i, oneMinutePrice);
                AssignTechAnalysis(fiveMinutesPrevValues, i, prevFiveMinutePrice);

                yield return (oneMinutePrice, prevFiveMinutePrice);
            }
        }

        private static void AssignTechAnalysis(Dictionary<int, decimal> prevValues, int i, PriceModel item)
        {
            foreach (var timePeriod in Periods)
            {
                var taValue = prevValues[timePeriod];
                if (i >= timePeriod)
                {
                    if (timePeriod == 3)
                    {
                        item.SMA3 = taValue;
                    }
                    else if (timePeriod == 20)
                    {
                        item.SMA20 = taValue;
                    }
                    else if (timePeriod == 70)
                    {
                        item.SMA70 = taValue;
                    }
                    else if (timePeriod == 150)
                    {
                        item.SMA150 = taValue;
                    }
                }
            }
        }

        private static Dictionary<int, decimal> GetPrevValues(
            Dictionary<int, decimal> prevValues,
            int i,
            decimal price)
        {
            if (i == 0)
            {
                prevValues = Periods.ToDictionary(x => x, x => price);
            }
            else
            {
                prevValues = Periods.ToDictionary(
                    x => x,
                    period => Alphas[period] * price + Weights[period] * prevValues[period]);
            }

            return prevValues;
        }
    }
}
