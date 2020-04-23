using RealTime.BL.Prices;
using System.Collections.Generic;

namespace RealTime.Console
{
    public static class DebugExtensions
    {
        public static string ToDebugString(this PriceModel price, PricesTimeInterval interval)
            => string.Format("{0:g} - {1}:  1- {2:F2}, 2- {3:F2}, 3- {4:F2}, 4- {5:F2}{6}",
                        price.Date,
                        interval.ToShortString(),
                        price.Open,
                        price.Close,
                        price.High,
                        price.Low,
                        GetTaString(price.TechAnalysis));

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
