using System.Collections.Generic;

namespace RealTime.BL.Brokers
{
    public class BrokerAuthRequest : BrokerBaseAuthRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string TradePin { get; set; }

        public string ChallengeId { get; set; }

        public string ChallengeCode { get; set; }

        public string MfaCode { get; set; }

        public string Key { get; set; }

        public string AuthToken { get; set; }

        /// <summary>
        /// Currently used for Stash only. Default value is Phone, if PhoneAndEmail
        /// specified, Stash will send OTP to both phone and email
        /// </summary>
        public MfaType MfaType { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }

    public enum MfaType
    {
        Phone,
        PhoneAndEmail
    }
}
