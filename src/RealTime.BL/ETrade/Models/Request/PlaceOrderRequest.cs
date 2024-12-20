using System.Xml.Serialization;

namespace RealTime.BL.ETrade.Models.Request
{
    public class PlaceOrderRequest : PreviewOrderRequest
    {
        [XmlElement("PreviewIds")]
        public PreviewId[] PreviewIds { get; set; }
    }

    public class PreviewId
    {
        [XmlElement("PreviewId")]
        public long PreviewIdValue { get; set; }
        [XmlElement("CashMargin")]
        public string CashMargin { get; set; }
    }
}
