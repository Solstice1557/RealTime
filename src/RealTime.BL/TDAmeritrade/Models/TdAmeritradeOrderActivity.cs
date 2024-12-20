using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeOrderActivity
    {
        [JsonPropertyName("activityType")]
        public TdAmeritradeOrderActivityTypeEnum? ActivityType { get; set; }
        
        [JsonPropertyName("executionType")]
        public TdAmeritradeExecutionTypeEnum? ExecutionType { get; set; }
        
        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }
        
        [JsonPropertyName("orderRemainingQuantity")]
        public decimal? OrderRemainingQuantity { get; set; }
        
        [JsonPropertyName("executionLegs")]
        public TdAmeritradeExecutionLeg[] ExecutionLegs { get; set; }
    }
}