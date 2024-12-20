using System.Collections.Generic;

namespace RealTime.BL.Brokers
{
    public class BrokerPortfolioModel
    {
        public BrokerType Type { get; set; }
        public BrokerBalance Balance { get; set; }
        public BrokerPortfolioRequestStatus Status { get; set; }
        public string ErrorMessage { get; set; }

        public IReadOnlyCollection<BrokerPosition> Positions { get; set; }
    }

    public class BrokerPosition
    {
        /// <summary>
        /// stock symbol name
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Amount of the stock position
        /// 16 decimal positions and 3 fractional positions
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The total original value (or purchase price) of the stock
        /// </summary>
        public decimal? CostBasis { get; set; }
    }

    public enum BrokerPortfolioRequestStatus
    {
        Succeeded,
        Failed,
        NotAuthorized
    }
}
