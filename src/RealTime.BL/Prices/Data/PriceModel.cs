namespace RealTime.BL.Prices
{
    using System;
    using System.Collections.Generic;

    public class PriceModel
    {
        public DateTime Date { get; set; }

        public decimal? Open { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Close { get; set; }

        public decimal? SMA3 { get; set; }

        public decimal? SMA20 { get; set; }

        public decimal? SMA70 { get; set; }

        public decimal? SMA150 { get; set; }

        public decimal? Volume { get; set; }

        public Dictionary<string, decimal?> TechAnalysis { get; } = new Dictionary<string, decimal?>();

        public PriceModel Clone()
            =>
            new PriceModel
            {
                Date = Date,
                Open = Open,
                High = High,
                Low = Low,
                Close = Close
            };
    }
}
