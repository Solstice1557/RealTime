using Newtonsoft.Json;
using System;

namespace RealTime.BL.Polygon
{
    public sealed class AggregatedPrice
    {
        private DateTime? _startTime;

        private DateTime? _endTime;

        [JsonProperty(PropertyName = "sym", Required = Required.Always)]
        public string Symbol { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "o", Required = Required.Always)]
        public decimal Open { get; set; }

        [JsonProperty(PropertyName = "h", Required = Required.Always)]
        public decimal High { get; set; }

        [JsonProperty(PropertyName = "l", Required = Required.Always)]
        public decimal Low { get; set; }

        [JsonProperty(PropertyName = "c", Required = Required.Always)]
        public decimal Close { get; set; }

        [JsonProperty(PropertyName = "a", Required = Required.Default)]
        public decimal Average { get; set; }

        [JsonProperty(PropertyName = "v", Required = Required.Always)]
        public long Volume { get; set; }

        [JsonProperty(PropertyName = "s", Required = Required.Always)]
        public long StartTimeOffset { get; set; }

        [JsonProperty(PropertyName = "e", Required = Required.Always)]
        public long EndTimeOffset { get; set; }

        [JsonProperty(PropertyName = "n", Required = Required.Default)]
        public int ItemsInWindow { get; set; }

        [JsonIgnore]
        public DateTime EndTime => _endTime ??
            (_endTime = ConvertToDateTime(EndTimeOffset)).Value;

        [JsonIgnore]
        public DateTime StartTime => _startTime ??
            (_startTime = ConvertToDateTime(StartTimeOffset)).Value;

        private static DateTime ConvertToDateTime(long offset)
        {
            return DateTime.SpecifyKind(
                DateTimeOffset.FromUnixTimeMilliseconds(offset).DateTime,
                DateTimeKind.Utc);
        }
    }
}
