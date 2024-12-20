using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerBaseAuthRequest
    {
        [Required]
        public BrokerType Type { get; set; }
    }
}
