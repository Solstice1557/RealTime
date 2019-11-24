namespace RealTime.BL.Prices
{
    public enum PricesTimeInterval : byte
    {
        Intraday1Min = 0,
        Intraday5Min = 1,
        Intraday15Min = 2,
        Intraday30Min = 3,
        Intraday1Hour = 4,
        Daily = 5,
        Weekly = 6,
        Monthly = 7
    }
}
