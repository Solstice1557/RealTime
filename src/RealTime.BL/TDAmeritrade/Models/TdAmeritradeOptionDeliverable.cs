using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeOptionDeliverable
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        
        [JsonPropertyName("deliverableUnits")]
        public decimal? DeliverableUnits { get; set; }
        
        [JsonPropertyName("currencyType")]
        public TdAmeritradeCurrencyTypeEnum? CurrencyType { get; set; }
        
        [JsonPropertyName("assetType")]
        public TdAmeritradeAssetType? AssetType { get; set; }
    }
}
