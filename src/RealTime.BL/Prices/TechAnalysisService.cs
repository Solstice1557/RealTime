namespace RealTime.BL.Prices
{
    using System;
    using System.Collections.Generic;

    public class TechAnalysisService : ITechAnalysisService
    {
        public Dictionary<DateTime, decimal?> CaclulateMovingAvg(List<(DateTime, decimal)> prices, int timePeriod)
        {
            if (timePeriod > prices.Count)
            {
                return new Dictionary<DateTime, decimal?>();
            }

            var sum = 0.0m;
            var result = new Dictionary<DateTime, decimal?>(prices.Count);
            for (int i = 0; i < prices.Count; i++)
            {
                sum += prices[i].Item2;
                if (i == timePeriod - 1)
                {
                    result.Add(prices[i].Item1, sum / timePeriod);
                }
                else if (i >= timePeriod)
                {
                    sum = sum - prices[i - timePeriod].Item2;
                    result.Add(prices[i].Item1, sum / timePeriod);
                }
                else
                {
                    result.Add(prices[i].Item1, null);
                }
            }

            return result;
        }

        public Dictionary<DateTime, decimal?> CaclulateSmoothedMovingAvg(
            List<(DateTime, decimal)> prices,
            int timePeriod)
        {
            var alpha = 1.0 / timePeriod;
            return CalculateExponentalMovingAverage(prices, timePeriod, alpha);
        }

        public Dictionary<DateTime, decimal?> CaclulateExponentalMovingAvg(
            List<(DateTime, decimal)> prices,
            int timePeriod)
        {
            var alpha = 2.0 / (timePeriod + 1);
            return CalculateExponentalMovingAverage(prices, timePeriod, alpha);
        }

        private static Dictionary<DateTime, decimal?> CalculateExponentalMovingAverage(
            List<(DateTime, decimal)> prices, 
            int timePeriod,
            double alpha)
        {
            if (timePeriod > prices.Count)
            {
                return new Dictionary<DateTime, decimal?>();
            }
            
            var weight = 1 - alpha;
            var previousResult = 0.0;
            var result = new Dictionary<DateTime, decimal?>(prices.Count);
            for (int i = 0; i < prices.Count; i++)
            {
                var price = (double)prices[i].Item2;
                if (i == 0)
                {
                    previousResult = price;
                }
                else
                {
                    previousResult = alpha * price + weight * previousResult;
                }

                if (i >= timePeriod)
                {
                    result.Add(prices[i].Item1, (decimal)previousResult);
                }
                else
                {
                    result.Add(prices[i].Item1, null);
                }
            }

            return result;
        }
    }
}
