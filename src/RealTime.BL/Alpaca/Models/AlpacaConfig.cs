namespace RealTime.BL.Alpaca.Models
{
    public class AlpacaConfig
    {
        public string AlpacaClientId { get; set; }
        public string AlpacaClientSecret { get; set; }
        public string AlpacaOAuthRedirectLink { get; set; } = "https://front.org";
    }
}
