namespace RealTime.BL.Prices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public abstract class BasePricesService : IPricesService
    {
        private readonly ITechAnalysisService techAnalysisService;

        protected BasePricesService(ITechAnalysisService techAnalysisService)
        {
            this.techAnalysisService = techAnalysisService;
        }

        public virtual async Task<List<PriceModel>> GetPrices(
            string symbol,
            PricesTimeInterval interval,
            int size,
            DateTime? fromDate,
            DateTime? toDate,
            params TechAnalysisInfo[] analyses)
        {
            var sizeShift = analyses != null && analyses.Length > 0 ? analyses.Max(x => x.TimePeriod) : 0;
            var prices = await this.LoadPrices(symbol, interval, size + sizeShift, fromDate, toDate);
            prices = prices.OrderBy(p => p.Date).ToList();
            this.LoadTechAnalyses(analyses, prices);

            if (prices.Count > size)
            {
                prices = prices.Skip(prices.Count - size).ToList();
            }

            prices.Reverse();
            return prices;
        }

        public virtual async Task<List<Dictionary<PricesTimeInterval, PriceModel>>> GetPrices(
            string symbol,
            PricesTimeInterval[] intervals,
            int size,
            DateTime? fromDate,
            DateTime? toDate,
            params TechAnalysisInfo[] analyses)
        {
            var sizeShift = analyses != null && analyses.Length > 0 ? analyses.Max(x => x.TimePeriod) : 0;
            intervals = intervals.Distinct().OrderBy(x => x).ToArray();
            var intervalPrices = new Dictionary<PricesTimeInterval, List<PriceModel>>();
            var lowestInterval = intervals[0];
            foreach (var interval in intervals)
            {
                var adjustedSize = GetAdjustedSize(size, interval, lowestInterval);

                var prices = await this.LoadPrices(symbol, interval, adjustedSize + sizeShift, fromDate, toDate);
                prices = prices.OrderBy(p => p.Date).ToList();
                this.LoadTechAnalyses(analyses, prices);

                if (prices.Count > size)
                {
                    prices = prices.Skip(prices.Count - size).ToList();
                }

                prices.Reverse();
                intervalPrices.Add(interval, prices);
            }

            var result = new List<Dictionary<PricesTimeInterval, PriceModel>>();
            foreach (var lowestIntervalPrice in intervalPrices[lowestInterval])
            {
                var dict = new Dictionary<PricesTimeInterval, PriceModel>();
                dict.Add(lowestInterval, lowestIntervalPrice);
                foreach (var interval in intervals)
                {
                    if (interval == lowestInterval)
                    {
                        continue;
                    }

                    var price = intervalPrices[interval]
                        .Where(p => p.Date <= lowestIntervalPrice.Date)
                        .OrderByDescending(p => p.Date)
                        .FirstOrDefault();
                    if (price != null)
                    {
                        dict.Add(interval, price);
                    }
                }

                result.Add(dict);
            }

            return result;
        }

        protected abstract Task<List<PriceModel>> LoadPrices(string symbol,
                                                             PricesTimeInterval interval,
                                                             int size,
                                                             DateTime? fromDate,
                                                             DateTime? toDate);

        protected static IEnumerable<PriceModel> GroupPricesByMinutes(
            IEnumerable<PriceModel> prices,
            int minutes)
        {
            var startDate = new DateTime(1970, 1, 1);
            return GroupPrices(prices, p => ((int)(p.Date - startDate).TotalMinutes) / minutes);
        }

        protected static IEnumerable<PriceModel> GroupPrices<T>(
            IEnumerable<PriceModel> prices,
            Func<PriceModel, T> groupingFunc)
        {
            return prices.GroupBy(groupingFunc)
                .Select(
                g => new PriceModel
                {
                    Date = g.Min(x => x.Date),
                    Open = g.OrderBy(x => x.Date).First(x => x.Open != null && x.Open > 0).Open,
                    Close = g.OrderBy(x => x.Date).Last(x => x.Close != null && x.Close > 0).Close,
                    High = g.Max(x => x.High),
                    Low = g.Where(x => x.Low.HasValue && x.Low > 0).Min(x => x.Low),
                    Volume = g.First().Volume
                });
        }

        protected void LoadTechAnalyses(TechAnalysisInfo[] analyses, List<PriceModel> prices)
        {
            if (analyses == null || analyses.Length == 0 || prices.Count == 0)
            {
                return;
            }

            var techAnalysesResults = analyses.Select(
                ta =>
                    {
                        var results = this.LoadTechAnalysis(ta, prices);
                        return new { results, ta };
                    }).ToArray();
            foreach (var taResult in techAnalysesResults)
            {
                var dict = taResult.results;
                string techAnalysisIdentifier = taResult.ta.ToString();
                var isSMA = taResult.ta.Type == TechAnalysisType.ExponentalMovingAverage
                            || taResult.ta.Type == TechAnalysisType.SmoothedMovingAverage;
                var timePeriod = taResult.ta.TimePeriod;

                foreach (var priceModel in prices)
                {
                    var taValue = dict.ContainsKey(priceModel.Date) ? dict[priceModel.Date] : null;

                    if (isSMA && timePeriod == 3)
                    {
                        priceModel.SMA3 = taValue;
                    }
                    else if (isSMA && timePeriod == 20)
                    {
                        priceModel.SMA20 = taValue;
                    }
                    else if (isSMA && timePeriod == 70)
                    {
                        priceModel.SMA70 = taValue;
                    }
                    else if (isSMA && timePeriod == 150)
                    {
                        priceModel.SMA150 = taValue;
                    }

                    priceModel.TechAnalysis.Add(techAnalysisIdentifier, taValue);
                }
            }
        }

        private static int GetAdjustedSize(
            int size,
            PricesTimeInterval interval,
            PricesTimeInterval lowestInterval)
        {
            if (lowestInterval == interval)
            {
                return size;
            }

            var lowerIntervalMinutes = GetMinutesInInterval(lowestInterval);
            var intervalMinutes = GetMinutesInInterval(interval);

            return (size * lowerIntervalMinutes / intervalMinutes) + 1;
        }

        private static int GetMinutesInInterval(PricesTimeInterval interval)
        {
            switch (interval)
            {
                case PricesTimeInterval.Intraday1Min:
                    return 1;
                case PricesTimeInterval.Intraday3Min:
                    return 3;
                case PricesTimeInterval.Intraday5Min:
                    return 5;
                case PricesTimeInterval.Intraday15Min:
                    return 15;
                case PricesTimeInterval.Intraday30Min:
                    return 30;
                case PricesTimeInterval.Intraday1Hour:
                    return 60;
                case PricesTimeInterval.Daily:
                    return 60 * 6 + 30;
                case PricesTimeInterval.Weekly:
                    return (60 * 6 + 30) * 5;
                case PricesTimeInterval.Monthly:
                    return (60 * 6 + 30) * 22;
            }

            return 1;
        }

        private Dictionary<DateTime, decimal?> LoadTechAnalysis(
            TechAnalysisInfo analysis,
            List<PriceModel> prices)
        {
            var list = GetPricesList(prices, analysis.PriceType);
            switch (analysis.Type)
            {
                case TechAnalysisType.MovingAverage:
                    return this.techAnalysisService.CaclulateMovingAvg(list, analysis.TimePeriod);
                case TechAnalysisType.SmoothedMovingAverage:
                    return this.techAnalysisService.CaclulateSmoothedMovingAvg(list, analysis.TimePeriod);
                case TechAnalysisType.ExponentalMovingAverage:
                    return this.techAnalysisService.CaclulateExponentalMovingAvg(list, analysis.TimePeriod);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static List<(DateTime, decimal)> GetPricesList(List<PriceModel> prices, PriceType priceType)
        {
            List<(DateTime, decimal)> list;
            switch (priceType)
            {
                case PriceType.Open:
                    list = prices.Where(p => p.Open != null).Select(p => (p.Date, p.Open.Value)).ToList();
                    break;
                case PriceType.Close:
                    list = prices.Where(p => p.Close != null).Select(p => (p.Date, p.Close.Value)).ToList();
                    break;
                case PriceType.High:
                    list = prices.Where(p => p.High != null).Select(p => (p.Date, p.High.Value)).ToList();
                    break;
                case PriceType.Low:
                    list = prices.Where(p => p.Low != null).Select(p => (p.Date, p.Low.Value)).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(priceType), priceType, null);
            }

            return list;
        }
    }
}
