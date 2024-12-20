using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class GetAccountResponse
    {
        [JsonPropertyName("securitiesAccount")]
        public TdAmeritradeAccount SecuritiesAccount { get; set; }
    }
}
