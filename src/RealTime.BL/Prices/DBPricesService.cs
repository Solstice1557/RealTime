namespace RealTime.BL.Prices
{
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using RealTime.DAL;

    public class DBPricesService : BasePricesService
    {
        private readonly PricesDbContext pricesDbContext;

        public DBPricesService(
            PricesDbContext pricesDbContext,
            ITechAnalysisService techAnalysisService)
            : base(techAnalysisService)
        {
            this.pricesDbContext = pricesDbContext;
        }

        protected override async Task<List<PriceModel>> LoadPrices(
            string symbol,
            PricesTimeInterval interval,
            int size,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var fund = await this.pricesDbContext.Funds.FirstOrDefaultAsync(x => x.Symbol == symbol);
            if (fund == null)
            {
                return new List<PriceModel>();
            }

            return interval < PricesTimeInterval.Daily
                ? await LoadIntradayPrices(interval, size, fromDate, toDate, fund)
                : await LoadDailyPrices(interval, size, fromDate, toDate, fund);
        }

        private async Task<List<PriceModel>> LoadDailyPrices(
            PricesTimeInterval interval,
            int size,
            DateTime? fromDate,
            DateTime? toDate,
            DAL.Entities.Fund fund)
        {
            var dailyPricesQuery = this.pricesDbContext.DailyPrices.Where(p => p.FundId == fund.FundId);
            if (fromDate != null)
            {
                dailyPricesQuery = dailyPricesQuery.Where(p => p.Timestamp >= fromDate.Value);
            }

            if (toDate != null)
            {
                dailyPricesQuery = dailyPricesQuery.Where(p => p.Timestamp <= toDate.Value);
            }

            dailyPricesQuery = dailyPricesQuery.OrderByDescending(p => p.Timestamp);

            if (interval == PricesTimeInterval.Daily)
            {
                dailyPricesQuery = dailyPricesQuery.Take(size);
            }

            var prices = await dailyPricesQuery.Select(
                    p => new PriceModel
                    {
                        Open = p.Open,
                        Close = p.Close,
                        High = p.High,
                        Low = p.Low,
                        Date = p.Timestamp,
                        Volume = p.Volume
                    })
                .ToListAsync();
            if (interval == PricesTimeInterval.Weekly)
            {
                var startDate = new DateTime(1970, 1, 4); // some monday
                prices = GroupPrices(prices, p => (int)(p.Date - startDate).TotalDays / 7)
                    .OrderByDescending(p => p.Date)
                    .Take(size)
                    .ToList();
            }
            else if (interval == PricesTimeInterval.Monthly)
            {
                prices = GroupPrices(prices, p => p.Date.ToString("yyyyMM"))
                    .OrderByDescending(p => p.Date)
                    .Take(size)
                    .ToList();
            }

            return prices;
        }

        private async Task<List<PriceModel>> LoadIntradayPrices(
            PricesTimeInterval interval,
            int size,
            DateTime? fromDate,
            DateTime? toDate,
            DAL.Entities.Fund fund)
        {
            var groupByMinutes = 1;
            switch (interval)
            {
                case PricesTimeInterval.Intraday5Min:
                    groupByMinutes = 5;
                    break;
                case PricesTimeInterval.Intraday15Min:
                    groupByMinutes = 15;
                    break;
                case PricesTimeInterval.Intraday30Min:
                    groupByMinutes = 30;
                    break;
                case PricesTimeInterval.Intraday1Hour:
                    groupByMinutes = 60;
                    break;
            }

            var sizeFixed = groupByMinutes * size;
            var pricesQuery = this.pricesDbContext.Prices
                .Where(p => p.FundId == fund.FundId);
            if (fromDate != null)
            {
                pricesQuery = pricesQuery.Where(p => p.Timestamp >= fromDate.Value);
            }

            if (toDate != null)
            {
                pricesQuery = pricesQuery.Where(p => p.Timestamp <= toDate.Value);
            }

            var prices = await pricesQuery.OrderByDescending(p => p.Timestamp)
                .Take(sizeFixed)
                .Select(
                    p => new PriceModel
                    {
                        Open = p.Open,
                        Close = p.Close,
                        High = p.High,
                        Low = p.Low,
                        Date = p.Timestamp,
                        Volume = p.Volume
                    })
                .ToListAsync();

            if (groupByMinutes > 1)
            {
                prices = GroupPricesByMinutes(prices, groupByMinutes).ToList();
            }

            return prices;
        }
    }
}
