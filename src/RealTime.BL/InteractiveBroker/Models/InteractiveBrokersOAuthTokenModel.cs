namespace RealTime.BL.InteractiveBroker.Models
{
    public class InteractiveBrokersOAuthTokenModel
    {
        public string AccountId { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string LiveSessionToken { get; set; }
    }
}
