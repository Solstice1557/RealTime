namespace RealTime.BL.Alphavantage.Data
{
    public class AlphavantageFund
    {
        public string Symbol { get; set; }

        public string AssetType { get; set; }

        public string Name { get; set; }

        public string Exchange { get; set; }

        public long SharesOutstanding { get; set; }
    }
}
