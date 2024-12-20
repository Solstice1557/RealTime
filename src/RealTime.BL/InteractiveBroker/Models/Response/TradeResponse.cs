namespace RealTime.BL.InteractiveBroker.Models.Response
{
    public class TradeResponse
    {
        public string CustomerOrderId { get; set; }
        public string OrderId { get; set; }
        public string Ticker { get; set; }
        public string Side { get; set; }
        public string OrderType { get; set; }
        public string FilledQuantity { get; set; }
        public string TradePrice { get; set; }
        public string TradeSize { get; set; }
        public string AvgPrice { get; set; }
        public string ExecutionTime { get; set; }
        public string ExecId { get; set; }
    }
}
