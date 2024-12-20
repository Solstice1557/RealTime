using System.Threading.Tasks;

namespace RealTime.BL.Encryption
{
    public interface IKeyVaultService
    {
        Task<byte[]> SignRSA(byte[] data, string keyName);
        Task<byte[]> DecryptRSA(byte[] data, string keyName);
    }
}
