
namespace RealTime.BL.Brokers
{
    public class BrokerAuthPromptResponse
    {
        public BrokerAuthPromptStatus Status { get; set; }
        public string RedirectLink { get; set; }
        public string LinkToken { get; set; }
        public string OAuthToken { get; set; }
        public string ErrorMessage { get; set; }
    }
}
