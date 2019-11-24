namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AlphavantagePricesResponse
    {
        [JsonProperty("Meta Data")]
        public AlphavantagePricesResponseMetaData MetaData { get; set; }

        [JsonProperty("Error Message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("Time Series (5min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries5Min { get; set; }

        [JsonProperty("Time Series (1min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries1Min { get; set; }

        [JsonProperty("Time Series (15min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries15Min { get; set; }

        [JsonProperty("Time Series (30min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries30Min { get; set; }

        [JsonProperty("Time Series (60min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries60Min { get; set; }

        [JsonProperty("Time Series (Daily)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesDaily { get; set; }

        [JsonProperty("Weekly Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesWeekly { get; set; }

        [JsonProperty("Weekly Adjusted Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesWeeklyAdjusted { get; set; }

        [JsonProperty("Monthly Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesMonthly { get; set; }

        [JsonProperty("Monthly Adjusted Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesMonthlyAdjusted { get; set; }
    }
}
