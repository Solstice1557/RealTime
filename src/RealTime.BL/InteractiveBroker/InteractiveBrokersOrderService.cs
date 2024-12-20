using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Brokers;
using RealTime.BL.Encryption;
using RealTime.BL.Helpers;
using RealTime.BL.InteractiveBroker.Models;
using RealTime.BL.InteractiveBroker.Models.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RealTime.BL.InteractiveBroker
{
    public class InteractiveBrokersOrderService : BaseInteractiveBrokersService
    {
        public InteractiveBrokersOrderService(
            IOptions<InteractiveBrokersConfig> configOptions,
            HttpClient httpClient,
            ILogger<InteractiveBrokersOrderService> logger,
            IKeyVaultService keyVaultService)
            : base(configOptions.Value, httpClient, logger, keyVaultService)
        {
        }

        public async Task<BrokerCreateOrderResult> ExecuteOrder(
            string decryptedToken,
            string requestSymbol,
            decimal requestAmount,
            BrokerOrderType orderType,
            string exchangeSymbol)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/orders";

            if (string.IsNullOrEmpty(exchangeSymbol))
            {
                var errorMessage =
                    $"Could not execute Interactive Brokers order. Listing exchange for {requestSymbol} was not found";
                Logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var orderId = OAuth1Helper.GetNonce();

            var model = new Dictionary<string, string>()
            {
                {"customerOrderId", orderId},
                {"ticker", requestSymbol},
                {"listingExchange", exchangeSymbol},
                {"quantity", requestAmount.ToString(CultureInfo.InvariantCulture)},
                {"instrumentType", "STK"}, // STK means stock
                {"orderType", "Market"},
                {"timeInForce", "GTC"}, // Good till Cancel
                {"side", orderType == BrokerOrderType.Buy? "BUY" : "SELL"},
                {"outsideRTH", "1"} // Indicates if order is active outside regular trading hours
            };

            var header = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Post,
                true,
                model);

            var operationName = string.Format(BrokerOperationNames.PlaceOrder,
                orderType == BrokerOrderType.Buy ? "buy" : "sell");

            var order = await CreateAndExecuteHttpRequest<OrderConfirmationResponse>(
                header,
                url,
                operationName,
                HttpMethod.Post,
                false,
                JsonSerializerBehaviour.Default,
                model);
            var orderStatus = OrderStatus.Mapping[order.Status].ConvertStatus();

            return new BrokerCreateOrderResult
            {
                Status = orderStatus,
                Amount = requestAmount,
                BrokerType = BrokerType.InteractiveBrokers,
                OrderId = orderId
            };
        }

        public async Task<BrokerOrder> GetOrder(string decryptedToken, string requestId)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/orders/{requestId}";

            var header = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Get);
            var orders = await CreateAndExecuteHttpRequest<List<OrderConfirmationResponse>>(
                header,
                url,
                BrokerOperationNames.GetOrder,
                HttpMethod.Get);

            var order = orders.FirstOrDefault(x => x.CustomerOrderId == requestId);

            if (order == null)
            {
                return new BrokerOrder()
                {
                    Status = BrokerOrderStatus.Unknown
                };
            }

            return ConvertToBrokerOrder(order, tokenModel);
        }

        public async Task<BrokerOrderList> GetOrders(string decryptedToken)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/orders";

            var header = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Get);
            var orders = await CreateAndExecuteHttpRequest<List<OrderConfirmationResponse>>(
                header,
                url,
                BrokerOperationNames.GetOrders,
                HttpMethod.Get);

            var openOrders = orders.Select(x => ConvertToBrokerOrder(x, tokenModel)).ToList();
            var historicalOrders = await GetTrades(decryptedToken);

            // Combine historical and pending orders
            openOrders.AddRange(historicalOrders);

            return new BrokerOrderList
            {
                OrderList = openOrders
            };
        }

        //This method returns historical trades, only completed ones
        private async Task<IReadOnlyCollection<BrokerOrder>> GetTrades(string decryptedToken)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/trades";
            var startDate = DateTime.UtcNow.Date.AddDays(-6);

            var startDateText = startDate.ToString("yyyy-MM-dd");

            var getOrdersUrl = $"{url}?since={startDateText}";
            var model = new Dictionary<string, string>()
            {
                {"since", startDateText}
            };

            var header = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Get,
                true,
                model);
            var orders = await CreateAndExecuteHttpRequest<List<TradeResponse>>(
                header,
                getOrdersUrl,
                BrokerOperationNames.GetOrders,
                HttpMethod.Get,
                false,
                JsonSerializerBehaviour.Default);

            return orders.Select(
                order => new BrokerOrder
                {
                    Status = BrokerOrderStatus.Success,
                    Type = order.Side == "1" ? BrokerOrderType.Buy : BrokerOrderType.Sell,
                    Amount = Convert.ToDecimal(order.TradePrice, CultureInfo.InvariantCulture),
                    Symbol = order.Ticker,
                    Id = order.CustomerOrderId,
                    AccountId = tokenModel.AccountId,
                    ExecutionTime = GetTimestampFromDate(order.ExecutionTime)
                }).ToList();
        }

        public async Task CancelOrder(string decryptedToken, string requestId)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/orders/{requestId}";

            var header = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Delete);
            await CreateAndExecuteHttpRequest<OrderConfirmationResponse>(
                header,
                url,
                BrokerOperationNames.CancelOrder,
                HttpMethod.Delete);
        }

        private static BrokerOrder ConvertToBrokerOrder(OrderConfirmationResponse order,
            InteractiveBrokersOAuthTokenModel tokenModel)
        {
            return new BrokerOrder()
            {
                Status = OrderStatus.Mapping[order.Status].ConvertStatus(),
                Type = order.Side == "BUY" ? BrokerOrderType.Buy : BrokerOrderType.Sell,
                AccountId = tokenModel.AccountId,
                Amount = Convert.ToDecimal(order.Quantity, CultureInfo.InvariantCulture),
                Id = order.CustomerOrderId,
                Symbol = order.Ticker,
                ExecutionTime = GetTimestampFromDate(order.TransactionTime)
            };
        }

        private static DateTime? GetTimestampFromDate(string transactionTime)
        {
            if (DateTime.TryParseExact(
                    transactionTime,
                    "yyyyMMdd-HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var transactionDateTime))
            {
                return transactionDateTime;
            }

            return null;
        }
    }
}
