namespace RealTime.BL.ETrade.Models.Response
{
    public class Cash
    {
        public decimal FundsForOpenOrdersCash { get; set; }
        public decimal MoneyMktBalance { get; set; }
    }

    public class OpenCalls
    {
        public decimal CashCall { get; set; }
    }

    public class RealTimeValues
    {
        public decimal TotalAccountValue { get; set; }
        public decimal NetMv { get; set; }
        public decimal NetMvLong { get; set; }
    }

    public class Computed
    {
        public decimal CashAvailableForInvestment { get; set; }
        public decimal CashAvailableForWithdrawal { get; set; }
        public decimal NetCash { get; set; }
        public decimal CashBalance { get; set; }
        public decimal SettledCashForInvestment { get; set; }
        public decimal UnSettledCashForInvestment { get; set; }
        public decimal FundsWithheldFromPurchasePower { get; set; }
        public decimal FundsWithheldFromWithdrawal { get; set; }
        public decimal CashBuyingPower { get; set; }
        public decimal MarginBuyingPower { get; set; }
        public OpenCalls OpenCalls { get; set; }
        public RealTimeValues RealTimeValues { get; set; }
    }

    public class BalanceResponse
    {
        public string AccountId { get; set; }
        public string AccountType { get; set; }
        public string OptionLevel { get; set; }
        public string AccountDescription { get; set; }
        public Cash Cash { get; set; }
        public Computed Computed { get; set; }
    }

    public class BalanceResponseModel
    {
        public BalanceResponse BalanceResponse { get; set; }
    }
}
