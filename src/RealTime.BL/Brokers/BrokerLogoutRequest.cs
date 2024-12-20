using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerLogoutRequest
    {
        [Required]
        public BrokerType Type { get; set; }
    }
}
