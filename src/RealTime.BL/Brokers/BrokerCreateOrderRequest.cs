using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealTime.BL.Brokers
{
    public class BrokerCreateOrderRequest
    {
        [Required]
        public List<BrokerBaseRequest> BrokerList { get; set; }

        [Required]
        public string Symbol { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }   
}
