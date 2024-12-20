using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeOrderLegCollection
    {
        [JsonPropertyName("orderLegType")]
        public TdAmeritradeAssetType? OrderLegType { get; set; }
        
        [JsonPropertyName("legId")]
        public long? LegId { get; set; }
        
        [JsonPropertyName("instrument")]
        public TdAmeritradeInstrument Instrument { get; set; }
        
        [JsonPropertyName("instruction")]
        public TdAmeritradeInstructionEnum? Instruction { get; set; }
        
        [JsonPropertyName("positionEffect")]
        public TdAmeritradePositionEffectEnum? PositionEffect { get; set; }
        
        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }
        
        [JsonPropertyName("quantityType")]
        public TdAmeritradeQuantityTypeEnum? QuantityType { get; set; }
    }
}
