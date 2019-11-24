using System.Threading;
using System.Threading.Tasks;

namespace RealTime.BL.Sync
{
    public interface IPricesSyncronizer
    {
        Task SyncIntradayPrices(CancellationToken cancellationToken);
    }
}