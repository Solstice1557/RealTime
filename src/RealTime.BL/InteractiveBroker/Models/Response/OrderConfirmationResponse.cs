namespace RealTime.BL.InteractiveBroker.Models.Response
{
    public class OrderConfirmationResponse
    {
        public string CustomerOrderId { get; set; }
        public string OriginalCustomerOrderId { get; set; }
        public string ContractId { get; set; }
        public string Ticker { get; set; }
        public string Exchange { get; set; }
        public string ListingExchange { get; set; }
        public string RemainingQuantity { get; set; }
        public string FilledQuantity { get; set; }
        public string OrderType { get; set; }
        public string TimeInForce { get; set; }
        public string Side { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string LastPrice { get; set; }
        public string Status { get; set; }
        public string TransactionTime { get; set; }
    }
}
