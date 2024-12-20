using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace RealTime.BL.InteractiveBroker
{
    public static class LiveSessionTokenHelper
    {
        internal static string ComputeLiveSessionToken(
            string exponent,
            string oauthSecret,
            string diffieHellmanResponse,
            string primeString)
        {
            var exp = ParseHexStringToBigInteger(exponent);
            var prime = ParseHexStringToBigInteger(primeString);
            var dhParsed = ParseHexStringToBigInteger(diffieHellmanResponse);

            var k = GetLiveSessionTokenFromDiffieHellman(exp, dhParsed, prime);

            var liveSessionToken = HashDiffieHellmanKeyWithOauthSecret(k.ToByteArray(false, true), oauthSecret);

            return liveSessionToken;
        }

        internal static string SignWithLiveTokenHmacSha256(byte[] lst, byte[] sbs)
        {
            using var crypto = new HMACSHA256 { Key = lst };
            var hash = crypto.ComputeHash(sbs);
            var encoded = Convert.ToBase64String(hash);
            return Uri.EscapeDataString(encoded);
        }

        internal static string HashDiffieHellmanKeyWithOauthSecret(byte[] key, string secret)
        {
            using var crypto = new HMACSHA1 { Key = key };
            var data = Convert.FromBase64String(secret);
            var hash = crypto.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }

        internal static BigInteger ParseHexStringToBigInteger(string hexString)
        {
            if (!hexString.StartsWith("0"))
            {
                // BigInteger.Parse method interprets the highest-order bit of the first byte in value as the sign bit.
                // This is why we need to add 0 as the first digit (for this method to always consider a number as positive)
                hexString = "0" + hexString;
            }

            return BigInteger.Parse(hexString, NumberStyles.AllowHexSpecifier);
        }

        private static BigInteger GetLiveSessionTokenFromDiffieHellman(
            BigInteger randomExponent,
            BigInteger diffieHellmanIntegral,
            BigInteger prime)
        {
            return BigInteger.ModPow(diffieHellmanIntegral, randomExponent, prime);
        }
    }
}
