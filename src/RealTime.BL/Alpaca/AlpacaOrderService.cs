using System;
using System.Linq;
using System.Threading.Tasks;
using Alpaca.Markets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Alpaca.Models;
using RealTime.BL.Brokers;

namespace RealTime.BL.Alpaca
{
    public class AlpacaOrderService : BaseAlpacaMarketService, IAlpacaOrderService
    {
        public AlpacaOrderService(IOptions<AlpacaConfig> config, ILogger<AlpacaAuthService> logger)
            : base(config, logger) { }

        public async Task<BrokerCreateOrderResult> ExecuteOrder(
            string accessToken,
            string requestSymbol,
            decimal requestAmount,
            BrokerOrderType orderType)
        {
            var token = ParseToken(accessToken).Token;
            var operationName = string.Format(nameof(ExecuteOrder),
                orderType == BrokerOrderType.Buy ? "buy" : "sell");

            return await ExecuteAndHandleExceptions(async () =>
            {
                using var client = CreateAlpacaClient(token);

                //Alpaca API does not support fractional quantities
                if (orderType == BrokerOrderType.Buy && requestAmount % 1 != 0)
                {
                    var errMessage = "FractionalQuantitiesNotSupported Alpaca";
                    Logger.LogError("{Message} Supplied amount is {RequestAmount}.", errMessage, requestAmount);
                    throw new Exception(errMessage);
                }

                var amount = Convert.ToInt64(requestAmount);
                var order = new NewOrderRequest(
                    requestSymbol,
                    amount,
                    orderType == BrokerOrderType.Buy 
                        ? OrderSide.Buy
                        : OrderSide.Sell,
                    OrderType.Market,
                    TimeInForce.Day);

                var placedOrder = await client.PostOrderAsync(order);
                var orderDto = MapToBrokerOrder(placedOrder);
                return new BrokerCreateOrderResult()
                {
                    BrokerType = BrokerType.Alpaca,
                    OrderId = orderDto.Id,
                    Amount = requestAmount,
                    Status = orderDto.Status
                };
            }, operationName);
        }

        public async Task<BrokerOrderList> GetOrders(string accessToken)
        {
            var token = ParseToken(accessToken).Token;

            return await ExecuteAndHandleExceptions(async () =>
            {
                using var client = CreateAlpacaClient(token);
                var request = new ListOrdersRequest
                {
                    OrderStatusFilter = OrderStatusFilter.All
                };

                var orders = await client.ListOrdersAsync(request);
                var listOfOrders = orders.Select(MapToBrokerOrder);

                return new BrokerOrderList()
                {
                    OrderList = listOfOrders.ToArray()
                };

            }, nameof(GetOrders));
        }

        public async Task<BrokerOrder> GetOrder(string accessToken, string orderId)
        {
            var token = ParseToken(accessToken).Token;

            return await ExecuteAndHandleExceptions(async () =>
            {
                var orderGuid = ParseOrderGuid(orderId);

                using var client = CreateAlpacaClient(token);
                var order = await client.GetOrderAsync(orderGuid);
                return MapToBrokerOrder(order);
            }, nameof(GetOrder));
        }
        
        public async Task CancelOrder(string accessToken, string orderId)
        {
            var token = ParseToken(accessToken).Token;

            var result = await ExecuteAndHandleExceptions(async () =>
            {
                var orderGuid = ParseOrderGuid(orderId);

                using var client = CreateAlpacaClient(token);
                return await client.DeleteOrderAsync(orderGuid);
            }, nameof(CancelOrder));

            if (!result)
            {
                var message = "Count dont initiate canceling with Alpaca";
                Logger.LogError("{Message} Order id = {OrderId}", message, orderId);
                throw new Exception(message);
            }
        }

        private Guid ParseOrderGuid(string orderId)
        {
            var parseResult = Guid.TryParse(orderId, out var orderGuid);

            if (!parseResult)
            {
                var message = "Alpaca order Id must be a valid GUID. Provided Id is not correct";
                Logger.LogError(
                    "Error parsing guid order: {Message}. Provided Id: {OrderId}",
                    message,
                    orderId);
                throw new InvalidOperationException(message);
            }

            return orderGuid;
        }

        private static BrokerOrder MapToBrokerOrder(IOrder order)
        {
            return new BrokerOrder
            {
                Status = MapAlpacaStatus(order.OrderStatus),
                Symbol = order.Symbol,
                Amount = order.Quantity,
                Type = order.OrderSide == OrderSide.Sell ? BrokerOrderType.Sell : BrokerOrderType.Buy,
                StatusDetails = order.OrderStatus.ToString(),
                Id = order.OrderId.ToString(),
                Price = order.AverageFillPrice
            };
        }

        private static BrokerOrderStatus MapAlpacaStatus(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Accepted => BrokerOrderStatus.InProgress,
                OrderStatus.New => BrokerOrderStatus.InProgress,
                OrderStatus.PartialFill => BrokerOrderStatus.InProgress,
                OrderStatus.PartiallyFilled => BrokerOrderStatus.PartiallyFilled,
                OrderStatus.Filled => BrokerOrderStatus.Success,
                OrderStatus.DoneForDay => BrokerOrderStatus.InProgress,
                OrderStatus.Canceled => BrokerOrderStatus.Cancelled,
                OrderStatus.Replaced => BrokerOrderStatus.InProgress,
                OrderStatus.PendingCancel => BrokerOrderStatus.InProgress,
                OrderStatus.Stopped => BrokerOrderStatus.Failed,
                OrderStatus.Rejected => BrokerOrderStatus.Failed,
                OrderStatus.Suspended => BrokerOrderStatus.Failed,
                OrderStatus.PendingNew => BrokerOrderStatus.InProgress,
                OrderStatus.Calculated => BrokerOrderStatus.InProgress,
                OrderStatus.Expired => BrokerOrderStatus.Failed,
                OrderStatus.AcceptedForBidding => BrokerOrderStatus.InProgress,
                OrderStatus.PendingReplace => BrokerOrderStatus.InProgress,
                OrderStatus.Fill => BrokerOrderStatus.InProgress,
                OrderStatus.Held => BrokerOrderStatus.InProgress,
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }
    }
}
