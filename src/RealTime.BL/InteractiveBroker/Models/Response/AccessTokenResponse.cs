namespace RealTime.BL.InteractiveBroker.Models.Response
{
    public class AccessTokenResponse
    {
        public bool IsPaper { get; set; }
        public string OauthToken { get; set; }
        public string OauthTokenSecret { get; set; }
    }
}
