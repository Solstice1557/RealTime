namespace RealTime.DAL.Entities
{
    using System;

    public class PriceBase
    {
        public int FundId { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal? Open { get; set; }

        public decimal? Close { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Volume { get; set; }

        public virtual Fund Fund { get; set; }
    }
}
