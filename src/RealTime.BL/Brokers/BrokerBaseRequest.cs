using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerBaseRequest
    {
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public BrokerType Type { get; set; }
    }
}
