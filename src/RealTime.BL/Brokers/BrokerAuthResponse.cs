using System.Collections.Generic;

namespace RealTime.BL.Brokers
{
    public class BrokerAuthResponse
    {
        public BrokerAuthStatus Status { get; set; }

        public string ChallengeId { get; set; }

        public int? ChallengeExpiresInSeconds { get; set; }

        public string ErrorMessage { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public int? ExpiresInSeconds { get; set; }

        public int? RefreshTokenExpiresInSeconds { get; set; }

        public IReadOnlyCollection<BrokerAccountTokens> BrokerAccountTokens { get; set; }
    }
}
