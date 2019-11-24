using System.Threading;
using System.Threading.Tasks;

namespace RealTime.BL.Sync
{
    public interface IPricesSyncronizer
    {
        Task SyncIntradayPrices(CancellationToken cancellationToken);

        Task SyncDailyPrices(CancellationToken cancellationToken);

        Task SyncIntradayPrices(string[] symbols, CancellationToken cancellationToken);

        Task SyncDailyPrices(string[] symbols, CancellationToken cancellationToken);
    }
}