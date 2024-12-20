using System.Xml.Serialization;

namespace RealTime.BL.ETrade.Models.Request
{
    public class CancelOrderRequest
    {
        [XmlElement("orderId")]
        public long OrderId { get; set; }
    }
}
