using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerRefreshTokenRequest : BrokerBaseAuthRequest
    {
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// optional, used when we need to refresh resfresh token
        /// </summary>
        public bool? CreateNewRefreshToken { get; set; }

        /// <summary>
        /// Provide access token in case of WeBull.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
