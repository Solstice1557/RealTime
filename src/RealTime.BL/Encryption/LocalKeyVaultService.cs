using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RealTime.BL.Encryption
{
    public class LocalKeyVaultService : IKeyVaultService
    {
        private const string KeyNotFoundMessage = "Could not find local key file";

        public async Task<byte[]> SignRSA(byte[] data, string keyName)
        {
            var privateKeyBytes = await GetKeyBytes(keyName);

            using var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public async Task<byte[]> DecryptRSA(byte[] data, string keyName)
        {
            var privateKeyBytes = await GetKeyBytes(keyName);

            using var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        private async Task<byte[]> GetKeyBytes(string keyName)
        {
            var filename = Path.Combine(GetDataPath(), $"{keyName}.pem");
            if (!File.Exists(filename))
            {
                throw new InvalidOperationException(KeyNotFoundMessage);
            }

            var key = await File.ReadAllTextAsync(filename);
            var privateKeyBytes = FromPemFormat(key);
            return privateKeyBytes;
        }

        private string GetDataPath()
        {
            var assemblyFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            return Path.GetDirectoryName(Uri.UnescapeDataString(assemblyFile));
        }

        private static byte[] FromPemFormat(string pemKey)
        {
            pemKey = pemKey
                .Replace("-----BEGIN RSA PRIVATE KEY-----", string.Empty)
                .Replace("-----END RSA PRIVATE KEY-----", string.Empty)
                .Replace("-----BEGIN PRIVATE KEY-----", string.Empty)
                .Replace("-----BEGIN PUBLIC KEY-----", string.Empty)
                .Replace("-----END PRIVATE KEY-----", string.Empty)
                .Replace("-----END PUBLIC KEY-----", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty);

            return Convert.FromBase64String(pemKey);
        }
    }
}
