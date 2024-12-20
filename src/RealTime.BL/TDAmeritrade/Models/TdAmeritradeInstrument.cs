using System;
using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeInstrument
    {
        [JsonPropertyName("assetType")]
        public TdAmeritradeAssetType? AssetType { get; set; }

        [JsonPropertyName("cusip")]
        public string Cusip { get; set; }
        
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("maturityDate")]
        public DateTime? MaturityDate { get; set; }

        [JsonPropertyName("variableRate")]
        public decimal? VariableRate { get; set; }

        [JsonPropertyName("factor")]
        public decimal? Factor { get; set; }

        [JsonPropertyName("type")]
        public TdAmeritradeIntrumentTypeEnum? Type { get; set; }

        [JsonPropertyName("putCall")]
        public TdAmeritradeOptionPutCallEnum? PutCall { get; set; }

        [JsonPropertyName("underlyingSymbol")]
        public string UnderlyingSymbol { get; set; }

        [JsonPropertyName("optionMultiplier")]
        public int? OptionMultiplier { get; set; }

        [JsonPropertyName("optionDeliverables")]
        public TdAmeritradeOptionDeliverable[] OptionDeliverables { get; set; }
    }
}