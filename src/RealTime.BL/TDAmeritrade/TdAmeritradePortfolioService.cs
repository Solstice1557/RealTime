using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using RealTime.BL.Brokers;
using RealTime.BL.Tdameritrade.Models;
using RealTime.BL.Tdameritrade.Utils;

namespace RealTime.BL.Tdameritrade
{
    public class TdAmeritradePortfolioService
    {
        private readonly IMapper mapper;
        private readonly ITdAmeritradeTokenService tdAmeritradeTokenService;
        private readonly ITdAmeritradeHttpClientService tdAmeritradeHttpClientService;
        private readonly ILogger logger;

        private  string GetAccountsPositionsAddress(string accountId) => $"/v1/accounts/{accountId}?fields=positions";
        private  string GetOrdersAddress(string accountId) => $"/v1/accounts/{accountId}/orders";
        private string GetAccountAddress(string accountId) => $"/v1/accounts/{accountId}";
        private string ExecuteOrdersAddress(string accountId) => $"/v1/accounts/{accountId}/orders";
        private string GerOrderAddress(string accountId, string orderId) => $"/v1/accounts/{accountId}/orders/{orderId}";

        public TdAmeritradePortfolioService(
            IMapper mapper,
            ITdAmeritradeTokenService tdAmeritradeTokenService,
            ITdAmeritradeHttpClientService tdAmeritradeHttpClientService, 
            ILogger<TdAmeritradePortfolioService> logger)
        {
            this.mapper = mapper;
            this.tdAmeritradeTokenService = tdAmeritradeTokenService;
            this.tdAmeritradeHttpClientService = tdAmeritradeHttpClientService;
            this.logger = logger;
        }

        public async Task CancelOrder(
            string accountToken, 
            string orderId)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(accountToken);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Delete,
                GerOrderAddress(
                    accountId, 
                    orderId));

            await tdAmeritradeHttpClientService.Execute(
                httpRequest,
                accessToken);
        }

        public async Task<BrokerBalance> GetBalance(string token)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(token);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                GetAccountAddress(accountId));

            var account = await tdAmeritradeHttpClientService.Execute<GetAccountResponse>(
                httpRequest,
                accessToken);

            if (!account.SecuritiesAccount.CurrentBalances.BuyingPower.HasValue)
            {
                var message = "Could not get account's buying power from TdAmeritrade";
                logger.LogError(message);
                throw new Exception(message);
            }

            return new BrokerBalance
            {
                BuyingPower = account.SecuritiesAccount.CurrentBalances.BuyingPower.Value,
                Cash = account.SecuritiesAccount.CurrentBalances.CashBalance,
                Equity = account.SecuritiesAccount.CurrentBalances.Equity
                    - account.SecuritiesAccount.CurrentBalances.CashBalance
            };
        }

        public async Task ExecuteOrder(string accountToken, string symbol, decimal amount, BrokerOrderType orderType)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(accountToken);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                ExecuteOrdersAddress(
                    accountId));

            var order = new TdAmeritradeOrdersStrategy
            {
                OrderType = TdAmeritradeOrderType.MARKET,
                Session = TdAmeritradeSessionEnum.NORMAL,
                Duration = TdAmeritradeDurationEnum.DAY,
                OrderStrategyType = TdAmeritradeOrderStrategyTypeEnum.SINGLE,
                OrderLegCollection = new TdAmeritradeOrderLegCollection[]
                {
                    new TdAmeritradeOrderLegCollection
                    {
                        Instruction = orderType switch 
                        {
                            BrokerOrderType.Buy => TdAmeritradeInstructionEnum.BUY,
                            BrokerOrderType.Sell => TdAmeritradeInstructionEnum.SELL,
                            _ => throw new NotSupportedException("Unknown order execution instruction!")
                         },
                        Quantity = amount,
                        Instrument = new TdAmeritradeInstrument
                        {
                            Symbol = symbol,
                            AssetType = TdAmeritradeAssetType.EQUITY
                        }
                    },
                }
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.IgnoreNullValues = true;
            var content = JsonSerializer.Serialize(order, options);

            httpRequest.Content = new StringContent(
                content, 
                Encoding.UTF8, 
                "application/json");

            await tdAmeritradeHttpClientService.Execute(
                httpRequest,
                accessToken);
        }


        public async Task<IReadOnlyCollection<BrokerPosition>> GetPositions(
            string accountToken)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(accountToken);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                GetAccountsPositionsAddress(accountId));

            return (await tdAmeritradeHttpClientService.Execute<GetAccountResponse>(
                httpRequest,
                accessToken))
                .SecuritiesAccount
                .Positions
                .Where(x => x.Instrument.AssetType == TdAmeritradeAssetType.EQUITY)
                .Select(x => mapper.Map<BrokerPosition>(x))
                .ToList();
        }

        public async Task<BrokerOrder> GetOrder(
            string accountToken,
            string orderId)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(accountToken);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                GerOrderAddress(accountId,orderId));

            return mapper.Map<BrokerOrder>(
                await tdAmeritradeHttpClientService.Execute<TdAmeritradeOrdersStrategy>(
                httpRequest,
                accessToken));
        }

        public async Task<BrokerOrderList> GetOrders(
            string accountToken)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(accountToken);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                GetOrdersAddress(accountId)
                + $"?fromEnteredTime={DateTime.UtcNow.AddHours(-5).AddDays(-1):yyyy-MM-dd}"); //make EST (no daylight saving)
            var list = await tdAmeritradeHttpClientService.Execute<TdAmeritradeOrdersStrategy[]>(
                httpRequest,
                accessToken);

            return new BrokerOrderList
            {
                OrderList = list
                .Where(x => x.OrderLegCollection.FirstOrDefault()?.Instrument.AssetType == TdAmeritradeAssetType.EQUITY)
                .Select(x => mapper.Map<BrokerOrder>(x))
                .ToList()
            };
        }

        public async Task<BrokerAccount> GetAccount(
            string accountToken)
        {
            (var accountId, var accessToken) = tdAmeritradeTokenService.Split(accountToken);

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get, 
                GetAccountAddress(accountId));

            return mapper.Map<BrokerAccount>(await tdAmeritradeHttpClientService.Execute<GetAccountResponse>(
                httpRequest,
                accessToken));
        }
    }
}
