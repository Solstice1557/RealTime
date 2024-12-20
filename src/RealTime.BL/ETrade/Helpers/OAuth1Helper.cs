using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace RealTime.BL.ETrade.Helpers
{   
    public static class OAuth1Helper
    {
        private static readonly Random _random;
        private static readonly object _lock = new object();

        private const string Digit = "1234567890";
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";

        static OAuth1Helper()
        {
            _random = new Random();
        }

        public static SortedDictionary<string, string> GetBaseParameters(string consumerKey, string oauthToken = null)
        {
            var nonce = GetNonce();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            var parameters = new SortedDictionary<string, string>
            {
                {"oauth_callback", "oob"},
                {"oauth_consumer_key", consumerKey},
                {"oauth_nonce", nonce},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", timestamp},
                {"oauth_version", "1.0"}
            };

            if (!string.IsNullOrEmpty(oauthToken))
            {
                parameters.Add("oauth_token", Uri.EscapeDataString(oauthToken));
            }

            return parameters;
        }


        public static string CreateAuthorizationHeader(SortedDictionary<string, string> parameters, string signature)
        {
            parameters["oauth_signature"] = signature;
            var header = new StringBuilder("OAuth ");

            var oauthParameters = parameters.Where(x => x.Key.StartsWith("oauth")).ToList();
            var i = 0;
            foreach (var parameter in oauthParameters)
            {
                header.Append(parameter.Key).Append("=");
                header.Append('"').Append(parameter.Value).Append('"');

                if (++i < oauthParameters.Count)
                {
                    header.Append(",");
                }
            }

            return header.ToString();
        }


        public static string GetHmacSignature(string url, SortedDictionary<string, string> parameters, string consumerSecret, string tokenSecret, HttpMethod httpMethod)
        {
            var key = string.Concat(consumerSecret, "&", tokenSecret);
            var crypto = new HMACSHA1 { Key = Encoding.UTF8.GetBytes(key) };

            var signatureBase = GetSignatureBase(httpMethod, url, parameters);

            var data = Encoding.UTF8.GetBytes(signatureBase);
            var hash = crypto.ComputeHash(data);
            var hashBase64 = Convert.ToBase64String(hash);
            var encoded = Uri.EscapeDataString(hashBase64);
            return encoded;
        }

        private static string GetSignatureBase(HttpMethod httpMethod, string url, SortedDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();

            //'For' operator is not used for performance reasons
            var i = 0;
            foreach (var parameter in parameters)
            {
                sb.Append(parameter.Key);
                sb.Append("=");
                sb.Append(parameter.Value);

                if (++i < parameters.Count)
                {
                    sb.Append("&");
                }
            }


            var escapedUrl = Uri.EscapeDataString(url);
            var escapedParameters = Uri.EscapeDataString(sb.ToString());
            return $"{httpMethod.Method}&{escapedUrl}&{escapedParameters}";
        }

        internal static string GetNonce(byte length = 16)
        {
            const string chars = (Lower + Digit);

            var nonce = new char[length];
            lock (_lock)
            {
                for (var i = 0; i < nonce.Length; i++)
                {
                    nonce[i] = chars[_random.Next(0, chars.Length)];
                }
            }
            return new string(nonce);
        }
    }
}
