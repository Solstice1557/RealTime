using System;

namespace RealTime.BL.Prices
{
    public enum PricesTimeInterval : byte
    {
        Intraday1Min = 0,
        Intraday3Min = 1,
        Intraday5Min = 2,
        Intraday15Min = 3,
        Intraday30Min = 4,
        Intraday1Hour = 5,
        Daily = 6,
        Weekly = 7,
        Monthly = 8
    }

    public static class PricesTimeIntervalExtensions
    {
        public static string ToShortString(this PricesTimeInterval interval)
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
                    return "month";
            }

            throw new ArgumentOutOfRangeException(nameof(interval));
        }
    }
}
