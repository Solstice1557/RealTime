using Microsoft.Extensions.Options;
using RealTime.BL.Brokers;
using RealTime.BL.ETrade.Models;
using RealTime.BL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTime.BL.ETrade
{
    public class ETradeFacade
    {
        private readonly ETradeAuthService _authService;
        private readonly ETradePortfolioService _portfolioService;
        private readonly ETradeOrderService _orderService;
        private readonly IOptions<ETradeConfig> _configOptions;
        private string _oauthToken;

        public ETradeFacade(
            ETradeAuthService authService,
            ETradePortfolioService portfolioService,
            ETradeOrderService orderService,
            IOptions<ETradeConfig> configOptions)
        {
            _authService = authService;
            _portfolioService = portfolioService;
            _orderService = orderService;
            _configOptions = configOptions;
        }

        public void SetToken(string oauthToken)
        {
            _oauthToken = oauthToken;
        }

        public async Task<bool> Authenticate()
        {
            if (string.IsNullOrEmpty(_configOptions.Value.ConsumerKey)
                || string.IsNullOrEmpty(_configOptions.Value.ConsumerSecret))
            {
                throw new InvalidOperationException("No consumer key or secret specified in settings.");
            }

            var tokenData = await _authService.GetRequestToken();

            if (string.IsNullOrEmpty(tokenData.LinkToken))
            {
                Console.WriteLine("Failed to get redirect link.");
                return false;
            }

            BrowserHelper.OpenBrowser(tokenData.LinkToken);
            Console.WriteLine("Enter verification code:");
            var verifier = Console.ReadLine();

            var authResponse = await _authService.GetAccessToken(tokenData.OAuthToken, verifier);
            if (authResponse.Status == BrokerAuthStatus.Failed)
            {
                Console.WriteLine($"Failed to get token: {authResponse.ErrorMessage}");
                return false;
            }

            var accounts = authResponse.BrokerAccountTokens.ToArray();
            _oauthToken = accounts[0].AccessToken;
            if (accounts.Length > 1)
            {
                Console.WriteLine($"Please choose account:");

                for (var i = 0; i < accounts.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {accounts[i].AccountName}");
                }

                var ind = Console.ReadLine();
                if (!int.TryParse(ind, out var index) || index < 1 || index > accounts.Length)
                {
                    Console.WriteLine($"Invalid input");
                    return false;
                }

                _oauthToken = accounts[index - 1].AccessToken;
            }

            return true;
        }

        public async Task<IReadOnlyCollection<BrokerPosition>> GetPositions()
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            return await _portfolioService.GetPositions(_oauthToken);
        }

        public async Task<BrokerBalance> GetBalance()
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            return await _portfolioService.GetBalance(_oauthToken);
        }

        public async Task<IReadOnlyCollection<BrokerOrder>> GetOrders()
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            var ordersList = await _orderService.GetOrders(_oauthToken);
            return ordersList.OrderList;
        }

        public async Task CancelOrder(string id)
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            await _orderService.CancelOrder(_oauthToken, id);
        }

        public Task<string> Buy(string symbol, decimal amount)
            => ExecuteOrder(symbol, amount, BrokerOrderType.Buy);

        public Task<string> Sell(string symbol, decimal amount)
           => ExecuteOrder(symbol, amount, BrokerOrderType.Sell);

        private async Task<string> ExecuteOrder(string symbol, decimal amount, BrokerOrderType type)
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            var result = await _orderService.ExecuteOrder(_oauthToken, symbol, amount, type);
            return result.OrderId;
        }
    }
}
