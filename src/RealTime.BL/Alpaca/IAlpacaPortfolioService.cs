using RealTime.BL.Brokers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTime.BL.Alpaca
{
    public interface IAlpacaPortfolioService
    {
        Task<IReadOnlyCollection<BrokerPosition>> GetPositions(string accessToken);
        Task<BrokerBalance> GetBalance(string accessToken);
    }
}
