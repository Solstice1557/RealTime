namespace RealTime.PriceChangeMonitoring
{
    public class Performance
    {
        public Performance(
            string symbol,
            decimal performanceInPercent,
            decimal startPrice,
            decimal currentPrice)
        {
            this.Symbol = symbol;
            this.PerformanceInPercent = performanceInPercent;
            this.StartPrice = startPrice;
            this.CurrentPrice = currentPrice;
        }

        public string Symbol { get; }

        public decimal PerformanceInPercent { get; }

        public decimal StartPrice { get; }

        public decimal CurrentPrice { get; }
    }
}
