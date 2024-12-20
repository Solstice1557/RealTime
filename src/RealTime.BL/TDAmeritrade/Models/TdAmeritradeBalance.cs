using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    public class TdAmeritradeBalance
    {
        [JsonPropertyName("accruedInterest")]
        public decimal? AccruedInterest { get; set; }

        [JsonPropertyName("availableFunds")]
        public decimal? AvailableFunds { get; set; }

        [JsonPropertyName("availableFundsNonMarginableTrade")]
        public decimal? AvailableFundsNonMarginableTrade { get; set; }
        
        [JsonPropertyName("bondValue")]
        public decimal? BondValue { get; set; }
        
        [JsonPropertyName("buyingPower")]
        public decimal? BuyingPower { get; set; }

        [JsonPropertyName("buyingPowerNonMarginableTrade")]
        public decimal? BuyingPowerNonMarginableTrade { get; set; }

        [JsonPropertyName("cashBalance")]
        public decimal? CashBalance { get; set; }
        
        [JsonPropertyName("cashAvailableForTrading")]
        public decimal? CashAvailableForTrading { get; set; }
        
        [JsonPropertyName("cashReceipts")]
        public decimal? CashReceipts { get; set; }
        
        [JsonPropertyName("dayTradingBuyingPower")]
        public decimal? DayTradingBuyingPower { get; set; }
        
        [JsonPropertyName("dayTradingBuyingPowerCall")]
        public decimal? DayTradingBuyingPowerCall { get; set; }
        
        [JsonPropertyName("dayTradingEquityCall")]
        public decimal? DayTradingEquityCall { get; set; }
        
        [JsonPropertyName("equity")]
        public decimal? Equity { get; set; }
        
        [JsonPropertyName("equityPercentage")]
        public decimal? EquityPercentage { get; set; }
        
        [JsonPropertyName("liquidationValue")]
        public decimal? LiquidationValue { get; set; }
        
        [JsonPropertyName("longMarginValue")]
        public decimal? LongMarginValue { get; set; }

        [JsonPropertyName("longMarketValue")]
        public decimal? LongMarketValue { get; set; }

        [JsonPropertyName("longOptionMarketValue")]
        public decimal? LongOptionMarketValue { get; set; }
        
        [JsonPropertyName("longStockValue")]
        public decimal? LongStockValue { get; set; }
        
        [JsonPropertyName("maintenanceCall")]
        public decimal? MaintenanceCall { get; set; }
        
        [JsonPropertyName("maintenanceRequirement")]
        public decimal? MaintenanceRequirement { get; set; }
        
        [JsonPropertyName("margin")]
        public decimal? Margin { get; set; }

        [JsonPropertyName("marginBalance")]
        public decimal? MarginBalance { get; set; }

        [JsonPropertyName("marginEquity")]
        public decimal? MarginEquity { get; set; }
        
        [JsonPropertyName("moneyMarketFund")]
        public decimal? MoneyMarketFund { get; set; }
        
        [JsonPropertyName("mutualFundValue")]
        public decimal? MutualFundValue { get; set; }
        
        [JsonPropertyName("regTCall")]
        public decimal? RegTCall { get; set; }

        [JsonPropertyName("savings")]
        public decimal? Savings { get; set; }

        [JsonPropertyName("shortBalance")]
        public decimal? ShortBalance { get; set; }

        [JsonPropertyName("shortMarginValue")]
        public decimal? ShortMarginValue { get; set; }

        [JsonPropertyName("shortMarketValue")]
        public decimal? ShortMarketValue { get; set; }

        [JsonPropertyName("shortOptionMarketValue")]
        public decimal? ShortOptionMarketValue { get; set; }
        
        [JsonPropertyName("shortStockValue")]
        public decimal? ShortStockValue { get; set; }

        [JsonPropertyName("sma")]
        public decimal? Sma { get; set; }

        [JsonPropertyName("stockBuyingPower")]
        public decimal? StockBuyingPower { get; set; }

        [JsonPropertyName("optionBuyingPower")]
        public decimal? OptionBuyingPower { get; set; }

        [JsonPropertyName("totalCash")]
        public decimal? TotalCash { get; set; }
        
        [JsonPropertyName("isInCall")]
        public bool? IsInCall { get; set; }
        
        [JsonPropertyName("unsettledCash")]
        public decimal? UnsettledCash { get; set; }
        
        [JsonPropertyName("pendingDeposits")]
        public decimal? PendingDeposits { get; set; }
        
        [JsonPropertyName("accountValue")]
        public decimal? AccountValue { get; set; }
    }
}
