using RealTime.BL.Brokers;
using System.Collections.Generic;

namespace RealTime.BL.InteractiveBroker.Models
{
    public static class OrderStatus
    {
        public static IReadOnlyDictionary<string, InteractiveBrokersOrderStatus> Mapping = new Dictionary<string, InteractiveBrokersOrderStatus>()
        {
            ["-1"] =InteractiveBrokersOrderStatus.InvalidOrderStatus,
            ["0"] = InteractiveBrokersOrderStatus.New,
            ["1"] = InteractiveBrokersOrderStatus.PartiallyFilled,
            ["2"] = InteractiveBrokersOrderStatus.Filled,
            ["3"] = InteractiveBrokersOrderStatus.DoneForTheDay,
            ["4"] = InteractiveBrokersOrderStatus.Canceled,
            ["5"] = InteractiveBrokersOrderStatus.Replaced,
            ["6"] = InteractiveBrokersOrderStatus.PendingCancelReplace,
            ["7"] = InteractiveBrokersOrderStatus.Stopped,
            ["8"] = InteractiveBrokersOrderStatus.Rejected,
            ["9"] = InteractiveBrokersOrderStatus.Suspended,
            ["A"] = InteractiveBrokersOrderStatus.PendingNew,
            ["B"] = InteractiveBrokersOrderStatus.Calculated,
            ["C"] = InteractiveBrokersOrderStatus.Expired,
            ["D"] = InteractiveBrokersOrderStatus.AcceptedForBidding,
            ["E"] = InteractiveBrokersOrderStatus.PendingReplace,
        };

        internal static BrokerOrderStatus ConvertStatus(this InteractiveBrokersOrderStatus status)
        {
            return status switch
            {
                InteractiveBrokersOrderStatus.InvalidOrderStatus => BrokerOrderStatus.Failed,
                InteractiveBrokersOrderStatus.New => BrokerOrderStatus.InProgress,
                InteractiveBrokersOrderStatus.PartiallyFilled => BrokerOrderStatus.PartiallyFilled,
                InteractiveBrokersOrderStatus.Filled => BrokerOrderStatus.Success,
                InteractiveBrokersOrderStatus.DoneForTheDay => BrokerOrderStatus.InProgress,
                InteractiveBrokersOrderStatus.Canceled => BrokerOrderStatus.Cancelled,
                InteractiveBrokersOrderStatus.Replaced => BrokerOrderStatus.Cancelled,
                InteractiveBrokersOrderStatus.PendingCancelReplace => BrokerOrderStatus.InProgress,
                InteractiveBrokersOrderStatus.Stopped => BrokerOrderStatus.Failed,
                InteractiveBrokersOrderStatus.Rejected => BrokerOrderStatus.Failed,
                InteractiveBrokersOrderStatus.Suspended => BrokerOrderStatus.Failed,
                InteractiveBrokersOrderStatus.PendingNew => BrokerOrderStatus.InProgress,
                InteractiveBrokersOrderStatus.Calculated => BrokerOrderStatus.InProgress,
                InteractiveBrokersOrderStatus.Expired => BrokerOrderStatus.Failed,
                InteractiveBrokersOrderStatus.AcceptedForBidding => BrokerOrderStatus.InProgress,
                InteractiveBrokersOrderStatus.PendingReplace => BrokerOrderStatus.InProgress,
                _ => BrokerOrderStatus.InProgress
            };
        }
    }

    public enum InteractiveBrokersOrderStatus
    {
        InvalidOrderStatus,
        New,
        PartiallyFilled,
        Filled,
        DoneForTheDay,
        Canceled,
        Replaced,
        PendingCancelReplace,
        Stopped,
        Rejected,
        Suspended,
        PendingNew,
        Calculated,
        Expired,
        AcceptedForBidding,
        PendingReplace
    }
}
