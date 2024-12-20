using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradePosition
    {
        [JsonPropertyName("shortQuantity")]
        public decimal? ShortQuantity { get; set; }
        
        [JsonPropertyName("averagePrice")]
        public decimal? AveragePrice { get; set; }
        
        [JsonPropertyName("currentDayProfitLoss")]
        public decimal? CurrentDayProfitLoss { get; set; }
        
        [JsonPropertyName("currentDayProfitLossPercentage")]
        public decimal? CurrentDayProfitLossPercentage { get; set; }
        
        [JsonPropertyName("longQuantity")]
        public decimal? LongQuantity { get; set; }
        
        [JsonPropertyName("settledLongQuantity")]
        public decimal? SettledLongQuantity { get; set; }
        
        [JsonPropertyName("settledShortQuantity")]
        public decimal? SettledShortQuantity { get; set; }
        
        [JsonPropertyName("agedQuantity")]
        public decimal? AgedQuantity { get; set; }
        
        [JsonPropertyName("instrument")]
        public TdAmeritradeInstrument Instrument { get; set; }
        
        [JsonPropertyName("marketValue")]
        public decimal? MarketValue { get; set; }
    }
}