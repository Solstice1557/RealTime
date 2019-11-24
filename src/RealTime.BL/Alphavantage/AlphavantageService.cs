namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RestSharp;

    using RealTime.BL.Common;
    using RealTime.BL.Prices;

    using Microsoft.Extensions.Logging;

    public class AlphavantageService : IAlphavantageService
    {
        private const string BaseUrl = "https://www.alphavantage.co/query";

        private static readonly TimeSpan RetryTimespan = TimeSpan.FromSeconds(10);

        private readonly string apiKey;

        private readonly ILogger logger;

        public AlphavantageService(AppSettings settings, ILogger<AlphavantageService> logger)
        {
            this.apiKey = settings.AlphavantageApiKey;
            this.logger = logger;
        }

        public async Task<Dictionary<DateTime, AlphavantagePrice>> LoadPricesWithRetry(
            string symbol,
            PricesTimeInterval pricesInterval,
            bool full,
            CancellationToken cancellationToken,
            int retryCount = 5)
        {
            try
            {
                return await LoadPrices(symbol, pricesInterval, full, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch(Exception e)
            {
                retryCount--;
                if (retryCount == 0)
                {
                    throw;
                }

                this.logger.LogInformation(e, "Error connecting to Alphavantage. Waiting for retry.");
                await Task.Delay(RetryTimespan, cancellationToken);
                return await LoadPricesWithRetry(symbol, pricesInterval, full, cancellationToken, retryCount);
            }
        }

        public async Task<Dictionary<DateTime, AlphavantagePrice>> LoadPrices(
            string symbol,
            PricesTimeInterval pricesInterval,
            bool full,
            CancellationToken cancellationToken)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(Method.GET);
            (string function, string interval) = GetPricesIntervalParameters(pricesInterval);
            request.AddQueryParameter("function", function);
            request.AddQueryParameter("symbol", symbol.Replace(".", "-"));
            request.AddQueryParameter("outputsize", full ? "full" : "compact");
            if (!string.IsNullOrEmpty(interval))
            {
                request.AddQueryParameter("interval", interval);
            }

            request.AddQueryParameter("apikey", this.apiKey);
            var data = await client.GetAndParse<AlphavantagePricesResponse>(request, cancellationToken);
            if (!string.IsNullOrEmpty(data.ErrorMessage))
            {
                throw new Exception(data.ErrorMessage);
            }

            switch (pricesInterval)
            {
                case PricesTimeInterval.Intraday1Min:
                    return data.TimeSeries1Min;
                case PricesTimeInterval.Intraday5Min:
                    return data.TimeSeries5Min;
                case PricesTimeInterval.Intraday15Min:
                    return data.TimeSeries15Min;
                case PricesTimeInterval.Intraday30Min:
                    return data.TimeSeries30Min;
                case PricesTimeInterval.Intraday1Hour:
                    return data.TimeSeries60Min;
                case PricesTimeInterval.Daily:
                    return data.TimeSeriesDaily;
                case PricesTimeInterval.Weekly:
                    return data.TimeSeriesWeeklyAdjusted;
                case PricesTimeInterval.Monthly:
                    return data.TimeSeriesMonthlyAdjusted;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pricesInterval), pricesInterval, null);
            }
        }

        private static (string function, string interval) GetPricesIntervalParameters(PricesTimeInterval interval)
        {
            switch (interval)
            {
                case PricesTimeInterval.Intraday1Min:
                    return ("TIME_SERIES_INTRADAY", "1min");
                case PricesTimeInterval.Intraday5Min:
                    return ("TIME_SERIES_INTRADAY", "5min");
                case PricesTimeInterval.Intraday15Min:
                    return ("TIME_SERIES_INTRADAY", "15min");
                case PricesTimeInterval.Intraday30Min:
                    return ("TIME_SERIES_INTRADAY", "30min");
                case PricesTimeInterval.Intraday1Hour:
                    return ("TIME_SERIES_INTRADAY", "60min");
                case PricesTimeInterval.Daily:
                    return ("TIME_SERIES_DAILY_ADJUSTED", string.Empty);
                case PricesTimeInterval.Weekly:
                    return ("TIME_SERIES_WEEKLY_ADJUSTED", string.Empty);
                case PricesTimeInterval.Monthly:
                    return ("TIME_SERIES_MONTHLY_ADJUSTED", string.Empty);
                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }
        }
    }
}
