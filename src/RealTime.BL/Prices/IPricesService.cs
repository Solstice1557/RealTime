namespace RealTime.BL.Prices
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPricesService
    {
        Task<List<PriceModel>> GetPrices(
            string symbol,
            PricesTimeInterval interval,
            int size,
            DateTime? fromDate,
            DateTime? toDate,
            params TechAnalysisInfo[] analyses);
    }
}
