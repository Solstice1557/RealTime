using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeAccount
    {
        [JsonPropertyName("type")]
        public TdAmeritradeAccountTypeEnum? Type { get; set; }

        [JsonPropertyName("accountId")]
        public string AccountId { get; set; }

        [JsonPropertyName("roundTrips")]
        public int? RoundTrips { get; set; }

        [JsonPropertyName("isDayTrader")]
        public bool? IsDayTrader { get; set; }

        [JsonPropertyName("isClosingOnlyRestricted")]
        public bool? IsClosingOnlyRestricted { get; set; }

        [JsonPropertyName("positions")]
        public TdAmeritradePosition[] Positions { get; set; }

        [JsonPropertyName("orderStrategies")]
        public TdAmeritradeOrdersStrategy[] OrderStrategies { get; set; }
        
        [JsonPropertyName("initialBalances")]
        public TdAmeritradeBalance InitialBalances { get; set; }
        
        [JsonPropertyName("currentBalances")]
        public TdAmeritradeBalance CurrentBalances { get; set; }
        
        [JsonPropertyName("projectedBalances")]
        public TdAmeritradeBalance ProjectedBalances { get; set; }
    }

}
