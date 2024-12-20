
namespace RealTime.BL.InteractiveBroker.Models.Response
{
    public class SecurityResponse
    {
        public string ContractId { get; set; }
        public string Ticker { get; set; }
        public string Exchange { get; set; }
        public string ListingDivision { get; set; }
        public string SecurityType { get; set; }
        public string CompanyName { get; set; }
        public string Currency { get; set; }
        public decimal PriceIncr { get; set; }
    }
}
