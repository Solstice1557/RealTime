namespace RealTime.BL.Tdameritrade.Models
{
    public enum TdAmeritradeIntrumentTypeEnum
    {
      // Mutual Fund 
      NOT_APPLICABLE,
      OPEN_END_NON_TAXABLE,
      OPEN_END_TAXABLE,
      NO_LOAD_NON_TAXABLE,
      NO_LOAD_TAXABLE,

      // Cash equivalent
      SAVINGS,
      MONEY_MARKET_FUND,

      // Option
      VANILLA,
      BINARY,
      BARRIER
    }
}
