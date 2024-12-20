using System;
using System.Numerics;

namespace RealTime.BL.InteractiveBroker
{
    public static class DiffieHellmanHelper
    {
        private static readonly Random _random;
        private static readonly object _lock = new object();

        static DiffieHellmanHelper()
        {
            _random = new Random();
        }

        internal static BigInteger GetRandomBytes(byte size = 25)
        {
            lock (_lock)
            {
                var bytes = new byte[size];
                _random.NextBytes(bytes);
                // To make the sign bit always positive
                bytes[^1] &= 0x7F;
                return new BigInteger(bytes);
            }
        }

        internal static BigInteger GetLiveSessionTokenFromDiffieHellman(BigInteger randomExponent, BigInteger diffieHellmanIntegral, BigInteger prime)
        {
            return BigInteger.ModPow(diffieHellmanIntegral, randomExponent, prime);
        }

        internal static BigInteger GenerateDiffieHellman(BigInteger randomExponent, int generator, BigInteger prime)
        {
            return BigInteger.ModPow(generator, BigInteger.Abs(randomExponent), prime);
        }

        internal static string GetBase64InHex(string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
