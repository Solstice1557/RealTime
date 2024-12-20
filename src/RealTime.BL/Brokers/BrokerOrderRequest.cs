using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerOrderRequest : BrokerBaseRequest
    {
        [Required]
        public string Id { get; set; }
    }
}
