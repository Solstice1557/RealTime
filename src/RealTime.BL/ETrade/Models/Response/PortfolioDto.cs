using System.Collections.Generic;

namespace RealTime.BL.ETrade.Models.Response
{
    public class PortfolioDto
    {
        public PortfolioResponse PortfolioResponse { get; set; }
    }

    public class Product
    {
        public string Symbol { get; set; }
        public string SecurityType { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryDay { get; set; }
        public decimal StrikePrice { get; set; }
    }

    public class Quick
    {
        public decimal LastTrade { get; set; }
        public long LastTradeTime { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePct { get; set; }
        public decimal Volume { get; set; }
    }

    public class Position
    {
        public long PositionId { get; set; }
        public string SymbolDescription { get; set; }
        public long DateAcquired { get; set; }
        public decimal? PricePaid { get; set; }
        public decimal Commissions { get; set; }
        public decimal OtherFees { get; set; }
        public decimal Quantity { get; set; }
        public string PositionIndicator { get; set; }
        public string PositionType { get; set; }
        public decimal DaysGain { get; set; }
        public decimal DaysGainPct { get; set; }
        public decimal MarketValue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalGain { get; set; }
        public decimal TotalGainPct { get; set; }
        public decimal PctOfPortfolio { get; set; }
        public decimal CostPerShare { get; set; }
        public decimal TodayCommissions { get; set; }
        public decimal TodayFees { get; set; }
        public decimal TodayPricePaid { get; set; }
        public decimal TodayQuantity { get; set; }
        public string LotsDetails { get; set; }
        public string QuoteDetails { get; set; }
        public Product Product { get; set; }
        public Quick Quick { get; set; }
    }

    public class AccountPortfolio
    {
        public string AccountId { get; set; }
        public List<Position> Position { get; set; }
        public int TotalPages { get; set; }
    }

    public class PortfolioResponse
    {
        public List<AccountPortfolio> AccountPortfolio { get; set; }
    }
}
