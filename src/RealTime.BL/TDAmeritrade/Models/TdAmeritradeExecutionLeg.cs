using System;
using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeExecutionLeg
    {
        [JsonPropertyName("legId")]
        public int? LegId { get; set; }
        
        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }
        
        [JsonPropertyName("mismarkedQuantity")]
        public decimal? MismarkedQuantity { get; set; }
        
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        
        [JsonPropertyName("time")]
        public DateTime? Time { get; set; }
    }
}
