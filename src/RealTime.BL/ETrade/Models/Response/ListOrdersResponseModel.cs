using System.Collections.Generic;

namespace RealTime.BL.ETrade.Models.Response
{
    public class ListOrdersResponseModel
    {
        public OrdersResponse OrdersResponse { get; set; }
    }

    public class OrderDetail
    {
        public long PlacedTime { get; set; }
        public decimal OrderValue { get; set; }
        public string Status { get; set; }
        public string OrderTerm { get; set; }
        public string PriceType { get; set; }
        public decimal LimitPrice { get; set; }
        public decimal StopPrice { get; set; }
        public string MarketSession { get; set; }
        public bool AllOrNone { get; set; }
        public decimal NetPrice { get; set; }
        public decimal NetBid { get; set; }
        public decimal NetAsk { get; set; }
        public decimal Gcd { get; set; }
        public string Ratio { get; set; }
        public List<Instrument> Instrument { get; set; }
        public decimal? OrderNumber { get; set; }
        public decimal? BracketedLimitPrice { get; set; }
        public decimal? InitialStopPrice { get; set; }
        public long? ExecutedTime { get; set; }
    }

    
    public class OrderOverview
    {
        public long OrderId { get; set; }
        public string Details { get; set; }
        public string OrderType { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public decimal? TotalOrderValue { get; set; }
        public decimal? TotalCommission { get; set; }
    }

    public class OrdersResponse
    {
        public string Marker { get; set; }
        public string Next { get; set; }
        public List<OrderOverview> Order { get; set; }
    }
}
