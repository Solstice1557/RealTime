using System;

namespace RealTime.BL.Brokers
{
    public class BrokerOrder
    {
        public string Id { get; set; }

        public BrokerOrderType Type { get; set; }

        public string Symbol { get; set; }

        public decimal? Amount { get; set; }

        public string AccountId { get; set; }

        public BrokerOrderStatus Status { get; set; }

        public decimal? Price { get; set; }
       
        public string StatusDetails { get; set; }

        public DateTime? ExecutionTime { get; set; }

        public bool FailedToSellDueToInsufficientAmountOfStocks { get; set; }
    }
}
