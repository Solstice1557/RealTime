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
}
