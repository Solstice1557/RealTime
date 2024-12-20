namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using RealTime.BL.Alphavantage.Data;
    using RealTime.BL.Prices;

    public interface IAlphavantageService
    {
        Task<Dictionary<DateTime, AlphavantagePrice>> LoadPricesWithRetry(
            string symbol,
            PricesTimeInterval pricesInterval,
            bool full,
            CancellationToken cancellationToken,
            int retryCount = 5);

        Task<Dictionary<DateTime, AlphavantagePrice>> LoadPrices(
            string symbol,
            PricesTimeInterval interval,
            bool full,
            CancellationToken cancellationToken);

        Task<AlphavantageFund> GetFundOverview(string symbol);
    }
}