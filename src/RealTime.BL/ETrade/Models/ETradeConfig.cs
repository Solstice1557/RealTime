using System;

namespace RealTime.BL.ETrade.Models
{
    public class ETradeConfig
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public Uri Endpoint { get; set; }
        public string OAuthRedirectLink { get; set; }
    }
}
