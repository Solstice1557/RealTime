using System;
using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeOrdersStrategy
    {
        [JsonPropertyName("session")]
        public TdAmeritradeSessionEnum? Session { get; set; }
        
        [JsonPropertyName("duration")]
        public TdAmeritradeDurationEnum? Duration { get; set; }
        
        [JsonPropertyName("orderType")]
        public TdAmeritradeOrderType? OrderType { get; set; }
        
        [JsonPropertyName("cancelTime")]
        public TdAmeritradeCancelTime CancelTime { get; set; }
        
        [JsonPropertyName("complexOrderStrategyType")]
        public TdAmeritradeComplexOrderStrategyTypeEnum? ComplexOrderStrategyType { get; set; }
        
        [JsonPropertyName("quantity")]
        public decimal? Quantity { get; set; }
        
        [JsonPropertyName("filledQuantity")]
        public decimal? FilledQuantity { get; set; }
        
        [JsonPropertyName("remainingQuantity")]
        public decimal? RemainingQuantity { get; set; }
        
        [JsonPropertyName("requestedDestination")]
        public TdAmeritradeDestinationEnum? RequestedDestination { get; set; }
        
        [JsonPropertyName("destinationLinkName")]
        public string DestinationLinkName { get; set; }
        
        [JsonPropertyName("releaseTime")]
        public DateTime? ReleaseTime { get; set; }
        
        [JsonPropertyName("stopPrice")]
        public decimal? StopPrice { get; set; }
        
        [JsonPropertyName("stopPriceLinkBasis")]
        public TdAmeritradePriceLinkBasisEnum? StopPriceLinkBasis { get; set; }
        
        [JsonPropertyName("stopPriceLinkType")]
        public TdAmeritradePriceLinkTypeEnum? StopPriceLinkType { get; set; }
        
        [JsonPropertyName("stopPriceOffset")]
        public decimal? StopPriceOffset { get; set; }
        
        [JsonPropertyName("stopType")]
        public TdAmeritradeStopTypeEnum? StopType { get; set; }
        
        [JsonPropertyName("priceLinkBasis")]
        public TdAmeritradePriceLinkBasisEnum? PriceLinkBasis { get; set; }
        
        [JsonPropertyName("priceLinkType")]
        public TdAmeritradePriceLinkTypeEnum? PriceLinkType { get; set; }
        
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("taxLotMethod")]
        public TdAmeritradeTaxLotMethodEnum? TaxLotMethod { get; set; }
        
        [JsonPropertyName("orderLegCollection")]
        public TdAmeritradeOrderLegCollection[] OrderLegCollection { get; set; }
        
        [JsonPropertyName("activationPrice")]
        public decimal? ActivationPrice { get; set; }
        
        [JsonPropertyName("specialInstruction")]
        public TdAmeritradeSpecialInstructionEnum? SpecialInstruction { get; set; }
        
        [JsonPropertyName("orderStrategyType")]
        public TdAmeritradeOrderStrategyTypeEnum? OrderStrategyType { get; set; }
        
        [JsonPropertyName("orderId")]
        public long? OrderId { get; set; }
        
        [JsonPropertyName("cancelable")]
        public bool? Cancelable { get; set; }
        
        [JsonPropertyName("editable")]
        public bool? Editable { get; set; }
        
        [JsonPropertyName("status")]
        public TdAmeritradeOrderStatusEnum? Status { get; set; }
        
        [JsonPropertyName("enteredTime")]
        public DateTime? EnteredTime { get; set; }
        
        [JsonPropertyName("closeTime")]
        public DateTime? CloseTime { get; set; }
        
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        
        [JsonPropertyName("accountId")]
        public long? AccountId { get; set; }
        
        [JsonPropertyName("orderActivityCollection")]
        public TdAmeritradeOrderActivity[] OrderActivityCollection { get; set; }
        
        //[JsonPropertyName("replacingOrderCollection")]
        //public TdAmeritradeReplacingOrderCollection[] ReplacingOrderCollection { get; set; }
        
        //[JsonPropertyName("childOrderStrategies")]
        //public TdAmeritradeChildOrderStrategy[] ChildOrderStrategies { get; set; }
        
        [JsonPropertyName("statusDescription")]
        public string StatusDescription { get; set; }
    }
}
