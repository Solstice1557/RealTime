namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Text.Json.Serialization;

    public class AlphavantageTechnicalAnalysisMetaData
    {
        [JsonPropertyName("1: Symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("2: Indicator")]
        public string Indicator { get; set; }

        [JsonPropertyName("3: Last Refreshed")]
        public DateTime LastRefreshed { get; set; }

        [JsonPropertyName("4: Interval")]
        public string Interval { get; set; }

        [JsonPropertyName("5: Time Period")]
        public string TimePeriod { get; set; }

        [JsonPropertyName("6: Series Type")]
        public string SeriesType { get; set; }

        [JsonPropertyName("7: Time Zone")]
        public string TimeZone { get; set; }
    }
}