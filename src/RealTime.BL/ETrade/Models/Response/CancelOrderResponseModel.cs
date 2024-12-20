namespace RealTime.BL.ETrade.Models.Response
{
    public class CancelOrderResponseModel
    {
        public CancelOrderResponse CancelOrderResponse { get; set; }
    }
    
    public class CancelOrderResponse
    {
        public string AccountId { get; set; }
        public long OrderId { get; set; }
        public long CancelTime { get; set; }
        public Messages Messages { get; set; }
    }
}
