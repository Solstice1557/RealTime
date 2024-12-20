using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeCancelTime
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }
        
        [JsonPropertyName("shortFormat")]
        public bool? ShortFormat { get; set; }
    }
}
