using RealTime.BL.Brokers;
using System.Threading.Tasks;

namespace RealTime.BL.Alpaca
{
    public interface IAlpacaOrderService
    {
        Task<BrokerCreateOrderResult> ExecuteOrder(
            string accessToken,
            string requestSymbol,
            decimal requestAmount,
            BrokerOrderType orderType);
        Task<BrokerOrderList> GetOrders(string accessToken);
        Task<BrokerOrder> GetOrder(string accessToken, string orderId);
        Task CancelOrder(string accessToken, string orderId);
    }
}
