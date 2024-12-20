using RealTime.BL.Brokers;

namespace RealTime.BL.ETrade.Extensions
{
    public static class ETradeEnumsExtensions
    {
        internal static BrokerOrderStatus ConvertStatus(this string status)
        {
            return status switch
            {
                "OPEN" => BrokerOrderStatus.InProgress,
                "EXECUTED" => BrokerOrderStatus.Success,
                "CANCELLED" => BrokerOrderStatus.Cancelled,
                "INDIVIDUAL_FILLS" => BrokerOrderStatus.InProgress,
                "CANCEL_REQUESTED" => BrokerOrderStatus.InProgress,
                "EXPIRED" => BrokerOrderStatus.Cancelled,
                "REJECTED" => BrokerOrderStatus.Cancelled,
                "PARTIAL" => BrokerOrderStatus.InProgress,
                "OPTION_EXERCISE" => BrokerOrderStatus.InProgress,
                "OPTION_ASSIGNMENT" => BrokerOrderStatus.InProgress,
                "DO_NOT_EXERCISE" => BrokerOrderStatus.Failed,
                "DONE_TRADE_EXECUTED" => BrokerOrderStatus.Success,
                _ => BrokerOrderStatus.InProgress
            };
        }

        internal static BrokerOrderType ConvertOrderType(this string type)
        {
            return type switch
            {
                "BUY" => BrokerOrderType.Buy,
                "SELL" => BrokerOrderType.Sell,
                "BUY_TO_COVER" => BrokerOrderType.Buy,
                "SELL_SHORT" => BrokerOrderType.Sell,
                "BUY_OPEN" => BrokerOrderType.Buy,
                "BUY_CLOSE" => BrokerOrderType.Buy,
                "SELL_OPEN" => BrokerOrderType.Sell,
                "SELL_CLOSE" => BrokerOrderType.Sell,
                "EXCHANGE" => BrokerOrderType.Unknown,
                _ => BrokerOrderType.Unknown
            };
        }
    }
}
