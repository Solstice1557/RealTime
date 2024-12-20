using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Brokers;
using RealTime.BL.ETrade.Extensions;
using RealTime.BL.ETrade.Helpers;
using RealTime.BL.ETrade.Models;
using RealTime.BL.ETrade.Models.Request;
using RealTime.BL.ETrade.Models.Response;
using RealTime.BL.Helpers;
using Instrument = RealTime.BL.ETrade.Models.Request.Instrument;
using Order = RealTime.BL.ETrade.Models.Request.Order;
using PreviewId = RealTime.BL.ETrade.Models.Request.PreviewId;


namespace RealTime.BL.ETrade
{
    public class ETradeOrderService : BaseETradeService
    {
        public ETradeOrderService(
            IHttpClientFactory httpClientFactory,
            IOptions<ETradeConfig> configOptions,
            ILogger<ETradeOrderService> logger)
            : base(httpClientFactory, configOptions.Value, logger) { }

        public async Task<BrokerOrderList> GetOrders(string decryptedToken)
        {
            var token = GetOAuthTokens(decryptedToken);
            var orderList = new List<OrderOverview>();
            var result = await GetOrders(token, orderList, string.Empty);

            return new BrokerOrderList()
            {
                OrderList = result.Select(x =>
                {
                    var orderDetail = x.OrderDetail.First();
                    var instrument = orderDetail.Instrument.FirstOrDefault();
                    return new BrokerOrder()
                    {
                        Id = x.OrderId.ToString(),
                        Status = orderDetail.Status.ConvertStatus(),
                        StatusDetails = orderDetail.Status,
                        Amount = instrument.FilledQuantity,
                        Symbol = instrument.Product.Symbol,
                        Type = instrument.OrderAction.ConvertOrderType()
                    };
                }).ToList()
            };
        }

        public async Task<BrokerOrder> GetOrder(string decryptedToken, string requestId)
        {
            var allOrders = await GetOrders(decryptedToken);
            var order = allOrders.OrderList.FirstOrDefault(x => x.Id == requestId);

            if (order == null)
            {
                const string message = "Requested order not found.";
                Logger.LogError($"{message} Request id = {requestId}");
                throw new Exception(message);
            }

            return order;
        }

        public async Task<BrokerCreateOrderResult> ExecuteOrder(string decryptedToken, string requestSymbol, decimal requestAmount, BrokerOrderType orderType)
        {
            var token = GetOAuthTokens(decryptedToken);

            var order = new Order()
            {
                AllOrNone = "false",
                MarketSession = "REGULAR",
                OrderTerm = ETradeOrderTerm.GOOD_FOR_DAY.ToString(),
                PriceType = "MARKET",
                StopPrice = new int[] { },
                Instrument = new Instrument()
                {
                    OrderAction = orderType == BrokerOrderType.Buy
                        ? ETradeOrderAction.BUY.ToString()
                        : ETradeOrderAction.SELL.ToString(),
                    Quantity = requestAmount.ToString(),
                    QuantityType = "QUANTITY",
                    Product = new Models.Request.Product()
                    {
                        Symbol = requestSymbol,
                        SecurityType = EquityProductType
                    }
                }
            };

            var customerOrderId = GetUniqueETradeId();
            var previewOrderResponse = await PreviewOrder(token, order, customerOrderId);
            var placeOrderResponse = await PlaceOrder(token, previewOrderResponse.PreviewOrderResponse.PreviewIds.First().PreviewId, order, customerOrderId);

            var orderId = placeOrderResponse.PlaceOrderResponse.OrderIds.First().OrderId.ToString();
            var orderDetails = await GetOrder(decryptedToken, orderId);

            return new BrokerCreateOrderResult()
            {
                OrderId = orderId,
                Amount = requestAmount,
                BrokerType = BrokerType.ETrade,
                Status = orderDetails.Status
            };
        }

        public async Task CancelOrder(string decryptedToken, string requestId)
        {
            var token = GetOAuthTokens(decryptedToken);

            var accountKey = token.AccountId;
            var requestPath = $"v1/accounts/{accountKey}/orders/cancel.json";
            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, token.OAuthToken);

            var model = new CancelOrderRequest()
            {
                OrderId = long.Parse(requestId)
            };

            var content = model.ToXmlUtf8();

            var data = new ETradeRequest()
            {
                TokenSecret = token.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Put,
                Content = content
            };

            await GetETradeResponse(data);
        }

        private async Task<PlaceOrderResponseModel> PlaceOrder(ETradeOAuthToken token, long previewOrderId, Order order,
            string orderId)
        {
            var accountKey = token.AccountId;
            var requestPath = $"v1/accounts/{accountKey}/orders/place.json";
            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, token.OAuthToken);

            var model = new PlaceOrderRequest()
            {
                ClientOrderId = orderId,
                OrderType = EquityProductType,
                PreviewIds = new[]
                {
                    new PreviewId()
                    {
                        PreviewIdValue = previewOrderId
                    }
                },
                Order = order
            };

            var content = model.ToXmlUtf8();

            var data = new ETradeRequest()
            {
                TokenSecret = token.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Post,
                Content = content
            };

            var response = await GetETradeResponse(data);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseModel = responseContent.FromJson<PlaceOrderResponseModel>(JsonSerializerBehaviour.CaseInsensitive);

            return responseModel;
        }


        private async Task<List<OrderOverview>> GetOrders(ETradeOAuthToken tokens, List<OrderOverview> currentResult, string pagingMarker)
        {
            var requestPath = $"v1/accounts/{tokens.AccountId}/orders.json";

            while (true)
            {
                var queryParameters = new Dictionary<string, string>() { { "count", "100" } };

                if (!string.IsNullOrEmpty(pagingMarker))
                {
                    queryParameters.Add("marker", pagingMarker);
                }

                var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, tokens.OAuthToken);

                var data = new ETradeRequest()
                {
                    TokenSecret = tokens.OAuthTokenSecret,
                    RequestPath = requestPath,
                    Parameters = parameters,
                    HttpMethod = HttpMethod.Get,
                    QueryParameters = queryParameters
                };

                var response = await GetETradeResponse(data);

                var responseContent = await response.Content.ReadAsStringAsync();
                var ordersList = responseContent.FromJson<ListOrdersResponseModel>(JsonSerializerBehaviour.CaseInsensitive);

                if (ordersList == null)
                {
                    return currentResult;
                }

                currentResult.AddRange(ordersList.OrdersResponse.Order);

                //If paging marker is not empty, call the same method again, to add more orders to the list
                //until the marker is empty (which indicates that there are no more orders)
                if (!string.IsNullOrEmpty(ordersList.OrdersResponse.Marker))
                {
                    pagingMarker = ordersList.OrdersResponse.Marker;
                }
                else
                {
                    return currentResult;
                }
            }
        }

        private async Task<PreviewOrderResponseModel> PreviewOrder(ETradeOAuthToken token, Order order, string orderId)
        {
            var accountKey = token.AccountId;
            var requestPath = $"v1/accounts/{accountKey}/orders/preview.json";

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, token.OAuthToken);

            var model = new PreviewOrderRequest()
            {
                ClientOrderId = orderId,
                OrderType = EquityProductType,
                Order = order
            };

            var content = model.ToXmlUtf8();

            var data = new ETradeRequest()
            {
                TokenSecret = token.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Post,
                Content = content
            };

            var response = await GetETradeResponse(data);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseModel = responseContent.FromJson<PreviewOrderResponseModel>(JsonSerializerBehaviour.CaseInsensitive);

            return responseModel;
        }

    }
}
