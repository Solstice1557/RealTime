using System.Collections.Generic;

namespace RealTime.BL.ETrade.Models.Response
{
    public class PreviewOrderResponseModel
    {
        public PreviewOrderResponse PreviewOrderResponse { get; set; }
    }

    public class PreviewIdValue
    {
        public long PreviewId { get; set; }
    }

    public class Disclosure
    {
        public bool EhDisclosureFlag { get; set; }
        public bool ConditionalDisclosureFlag { get; set; }
        public bool AoDisclosureFlag { get; set; }
    }

    public class PreviewOrderResponse
    {
        public string OrderType { get; set; }
        public long PreviewTime { get; set; }
        public bool DstFlag { get; set; }
        public string AccountId { get; set; }
        public int OptionLevelCd { get; set; }
        public string MarginLevelCd { get; set; }
        public MessageList MessageList { get; set; }
        public List<Order> Order { get; set; }
        public List<PreviewIdValue> PreviewIds { get; set; }
        public PortfolioMargin PortfolioMargin { get; set; }
        public Disclosure Disclosure { get; set; }
    }
}