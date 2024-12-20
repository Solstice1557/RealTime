using RealTime.BL.Brokers;
using RealTime.BL.Helpers;
using RealTime.BL.Trading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTime.BL.InteractiveBroker
{
    public class InteractiveBrokersFacade
    {
        private readonly InteractiveBrokersAuthService authService;
        private readonly InteractiveBrokersPortfolioService portfolioService;
        private readonly InteractiveBrokersOrderService orderService;
        private readonly ExchangeSymbolService exchangeService;

        private string _oauthToken;

        public InteractiveBrokersFacade(
            InteractiveBrokersAuthService authService,
            InteractiveBrokersPortfolioService portfolioService,
            InteractiveBrokersOrderService orderService,
            ExchangeSymbolService exchangeService)
        {
            this.authService = authService;
            this.portfolioService = portfolioService;
            this.orderService = orderService;
            this.exchangeService = exchangeService;
        }

        public async Task<bool> Authenticate()
        {
            var tokenData = await authService.GetRequestToken();

            if (string.IsNullOrEmpty(tokenData.LinkToken))
            {
                Console.WriteLine("Failed to get redirect link.");
                return false;
            }

            BrowserHelper.OpenBrowser(tokenData.LinkToken);
            Console.WriteLine("Enter verification code:");
            var verifier = Console.ReadLine();

            var authResponse = await authService.GetAccessToken(tokenData.OAuthToken, verifier);
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

            return await portfolioService.GetPositions(_oauthToken);
        }

        public async Task<BrokerBalance> GetBalance()
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            return await portfolioService.GetBalance(_oauthToken);
        }

        public async Task<IReadOnlyCollection<BrokerOrder>> GetOrders()
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            var ordersList = await orderService.GetOrders(_oauthToken);
            return ordersList.OrderList;
        }

        public async Task CancelOrder(string id)
        {
            if (string.IsNullOrEmpty(_oauthToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            await orderService.CancelOrder(_oauthToken, id);
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

            var exchange = exchangeService.GetExchangeSymbol(symbol);
            var result = await orderService.ExecuteOrder(_oauthToken, symbol, amount, type, exchange);
            return result.OrderId;
        }
    }
}
