
namespace RealTime.BL.InteractiveBroker.Models.Response
{
    public class AccountSummary
    {
        public Summary Summary { get; set; }
    }

    public class Summary
    {
        public USDDetails USD { get; set; }
    }

    public class USDDetails
    {
        public decimal BuyingPower { get; set; }

        public decimal? TotalCashValue { get; set; }
    }
}
