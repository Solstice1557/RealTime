using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerSwitchAccountRequest : BrokerBaseAuthRequest
    {
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Provide access token in case of WeBull.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
