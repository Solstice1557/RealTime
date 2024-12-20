using System.Collections.Generic;

namespace RealTime.BL.Brokers
{
    public class BrokerOrderList
    {
        public IReadOnlyCollection<BrokerOrder> OrderList { get; set; }
    }
}
