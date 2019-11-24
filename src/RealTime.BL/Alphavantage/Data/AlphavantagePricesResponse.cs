namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class AlphavantagePricesResponse
    {
        [JsonPropertyName("Meta Data")]
        public AlphavantagePricesResponseMetaData MetaData { get; set; }

        [JsonPropertyName("Error Message")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("Time Series (5min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries5Min { get; set; }

        [JsonPropertyName("Time Series (1min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries1Min { get; set; }

        [JsonPropertyName("Time Series (15min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries15Min { get; set; }

        [JsonPropertyName("Time Series (30min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries30Min { get; set; }

        [JsonPropertyName("Time Series (60min)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeries60Min { get; set; }

        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesDaily { get; set; }

        [JsonPropertyName("Weekly Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesWeekly { get; set; }

        [JsonPropertyName("Weekly Adjusted Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesWeeklyAdjusted { get; set; }

        [JsonPropertyName("Monthly Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesMonthly { get; set; }

        [JsonPropertyName("Monthly Adjusted Time Series")]
        public Dictionary<DateTime, AlphavantagePrice> TimeSeriesMonthlyAdjusted { get; set; }
    }
}
