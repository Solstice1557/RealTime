using System;

namespace RealTime.BL.InteractiveBroker
{
    public class InteractiveBrokersConfig
    {
        public Uri Endpoint { get; set; }
        public string OAuthAuthLinkTemplate { get; set; }
        public string OAuthRedirectLink { get; set; }
        public string ConsumerKey { get; set; }
        public string KeyVaultEncryptionKeyName { get; set; }
        public string KeyVaultSignatureKeyName { get; set; }
        public string DiffieHellmanPrimeSecret { get; set; }
    }
}
