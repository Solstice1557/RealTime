using System.Collections.Generic;

namespace RealTime.BL.ETrade.Models.Response
{
    public class PlaceOrderResponseModel
    {
        public PlaceOrderResponse PlaceOrderResponse { get; set; }
    }

    public class MessageList { }

    public class Message
    {
        public string Description { get; set; }
        public int Code { get; set; }
        public string Type { get; set; }
    }

    public class Messages
    {
        public List<Message> Message { get; set; }
    }

    public class Instrument
    {
        public string SymbolDescription { get; set; }
        public string OrderAction { get; set; }
        public string QuantityType { get; set; }
        public decimal Quantity { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal FilledQuantity { get; set; }
        public decimal CancelQuantity { get; set; }
        public bool ReserveOrder { get; set; }
        public decimal ReserveQuantity { get; set; }
        public Product Product { get; set; }
    }

    public class Order
    {
        public string OrderTerm { get; set; }
        public string PriceType { get; set; }
        public decimal LimitPrice { get; set; }
        public decimal StopPrice { get; set; }
        public string MarketSession { get; set; }
        public bool AllOrNone { get; set; }
        public Messages Messages { get; set; }
        public string EgQual { get; set; }
        public double EstimatedCommission { get; set; }
        public double EstimatedFees { get; set; }
        public decimal NetPrice { get; set; }
        public decimal NetBid { get; set; }
        public decimal NetAsk { get; set; }
        public decimal Gcd { get; set; }
        public string Ratio { get; set; }
        public List<Instrument> Instrument { get; set; }
    }

    public class OrderIdValue
    {
        public long OrderId { get; set; }
    }

    public class PortfolioMargin
    {
        public double HouseExcessEquityNew { get; set; }
        public bool PmEligible { get; set; }
        public double HouseExcessEquityCurr { get; set; }
        public double HouseExcessEquityChange { get; set; }
    }

    public class PlaceOrderResponse
    {
        public string OrderType { get; set; }
        public bool DstFlag { get; set; }
        public int OptionLevelCd { get; set; }
        public string MarginLevelCd { get; set; }
        public long PlacedTime { get; set; }
        public string AccountId { get; set; }
        public MessageList MessageList { get; set; }
        public List<Order> Order { get; set; }
        public List<OrderIdValue> OrderIds { get; set; }
        public PortfolioMargin PortfolioMargin { get; set; }
    }
}
