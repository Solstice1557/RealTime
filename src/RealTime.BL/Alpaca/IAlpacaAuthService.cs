using RealTime.BL.Brokers;
using System.Threading.Tasks;

namespace RealTime.BL.Alpaca
{
    public interface IAlpacaAuthService
    {
        BrokerAuthPromptResponse GetLinkToken();
        Task<BrokerAuthResponse> Authenticate(string token);
        string GetAccountId(string decryptedToken);
    }
}
