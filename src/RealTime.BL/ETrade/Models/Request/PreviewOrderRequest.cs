using System.Xml.Serialization;

namespace RealTime.BL.ETrade.Models.Request
{
    public class PreviewOrderRequest
    {
        [XmlElement("orderType")]
        public string OrderType { get; set; }
        [XmlElement("clientOrderId")]
        public string ClientOrderId { get; set; }
        [XmlElement("Order")]
        public Order Order { get; set; }
    }

    public class Instrument
    {
        public Product Product { get; set; }
        [XmlElement("orderAction")]
        public string OrderAction { get; set; }
        [XmlElement("quantityType")]
        public string QuantityType { get; set; }
        [XmlElement("quantity")]
        public string Quantity { get; set; }
    }

    public class Product
    {
        [XmlElement("securityType")]
        public string SecurityType { get; set; }
        [XmlElement("symbol")]
        public string Symbol { get; set; }
    }

    public class Order
    {
        [XmlElement("allOrNone")]
        public string AllOrNone { get; set; }
        [XmlElement("priceType")]
        public string PriceType { get; set; }
        [XmlElement("orderTerm")]
        public string OrderTerm { get; set; }
        [XmlElement("marketSession")]
        public string MarketSession { get; set; }
        [XmlElement("stopPrice")]
        public int[] StopPrice { get; set; }
        [XmlElement("limitPrice")]
        public string LimitPrice { get; set; }
        [XmlElement("Instrument")]
        public Instrument Instrument { get; set; }
    }
}
