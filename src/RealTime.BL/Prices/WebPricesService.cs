namespace RealTime.BL.Prices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using RealTime.BL.Alphavantage;

    public class WebPricesService : BasePricesService
    {
        private readonly IAlphavantageService alphavantageService;

        public WebPricesService(
            IAlphavantageService alphavantageService,
            ITechAnalysisService techAnalysisService)
            : base(techAnalysisService)
        {
            this.alphavantageService = alphavantageService;
        }

        protected override async Task<List<PriceModel>> LoadPrices(
            string symbol,
            PricesTimeInterval interval,
            int size,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var loadedPrices = await this.alphavantageService.LoadPricesWithRetry(
                symbol,
                interval,
                size > 100,
                CancellationToken.None);

            var prices = loadedPrices.Select(
                kp => new PriceModel
                {
                    Date = kp.Key,
                    Close = kp.Value.Close.ToDecimal(),
                    Open = kp.Value.Open.ToDecimal(),
                    High = kp.Value.High.ToDecimal(),
                    Low = kp.Value.Low.ToDecimal(),
                    Volume = kp.Value.Volume.ToDecimal()
                })
                .OrderByDescending(p => p.Date)
                .ToList();
            return prices;
        }
    }
}
