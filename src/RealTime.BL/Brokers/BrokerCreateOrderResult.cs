using System;
using System.Collections.Generic;
using System.Text;

namespace RealTime.BL.Brokers
{
    public class BrokerCreateOrderResult
    {
        public BrokerType BrokerType { get; set; }
        public BrokerOrderStatus Status { get; set; }
        public string OrderId { get; set; }
        public string ErrorMessage { get; set; }
        public decimal Amount { get; set; }
    }
}
