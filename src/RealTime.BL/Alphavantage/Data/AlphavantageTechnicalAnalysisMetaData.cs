namespace RealTime.BL.Alphavantage
{
    using System;
    using Newtonsoft.Json;

    public class AlphavantageTechnicalAnalysisMetaData
    {
        [JsonProperty("1: Symbol")]
        public string Symbol { get; set; }

        [JsonProperty("2: Indicator")]
        public string Indicator { get; set; }

        [JsonProperty("3: Last Refreshed")]
        public DateTime LastRefreshed { get; set; }

        [JsonProperty("4: Interval")]
        public string Interval { get; set; }

        [JsonProperty("5: Time Period")]
        public string TimePeriod { get; set; }

        [JsonProperty("6: Series Type")]
        public string SeriesType { get; set; }

        [JsonProperty("7: Time Zone")]
        public string TimeZone { get; set; }
    }
}