using RealTime.BL.Brokers;
using RealTime.BL.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace RealTime.BL.Alpaca
{
    public class AlpacaBrokersFacade
    {
        private const string TokenFileName = "AlpacaRefreshToken.txt";

        private readonly IAlpacaPortfolioService _portfolioService;
        private readonly IAlpacaAuthService _authService;
        private readonly IAlpacaOrderService _orderService;
        private string _accessToken;

        public AlpacaBrokersFacade(
            IAlpacaPortfolioService portfolioService,
            IAlpacaAuthService authService,
            IAlpacaOrderService orderService)
        {
            this._portfolioService = portfolioService;
            this._authService = authService;
            this._orderService = orderService;
        }

        public async Task<bool> Authenticate()
        {
            if (File.Exists(TokenFileName))
            {
                _accessToken = File.ReadAllText(TokenFileName);

                if (!string.IsNullOrEmpty(_accessToken))
                {
                    return true;
                }
            }

            var authLink = _authService.GetLinkToken();

            BrowserHelper.OpenBrowser(authLink.LinkToken);
            Console.WriteLine("Enter auth code:");
            var authCode = Console.ReadLine();
            authCode = DecodeUrlString(authCode);

            var authResponse = await _authService.Authenticate(authCode);
            if (authResponse.Status == BrokerAuthStatus.Failed)
            {
                Console.WriteLine($"Failed to get token: {authResponse.ErrorMessage}");
                return false;
            }

            _accessToken = authResponse.AccessToken;
            File.WriteAllText(TokenFileName, _accessToken);

            return true;
        }

        public async Task<IReadOnlyCollection<BrokerPosition>> GetPositions()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            return await _portfolioService.GetPositions(_accessToken);
        }

        public async Task<BrokerBalance> GetBalance()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            return await _portfolioService.GetBalance(_accessToken);
        }

        public async Task<IReadOnlyCollection<BrokerOrder>> GetOrders()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            var ordersList = await _orderService.GetOrders(_accessToken);
            return ordersList.OrderList;
        }

        public async Task CancelOrder(string id)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            await _orderService.CancelOrder(_accessToken, id);
        }

        public Task<string> Buy(string symbol, decimal amount)
            => ExecuteOrder(symbol, amount, BrokerOrderType.Buy);

        public Task<string> Sell(string symbol, decimal amount)
           => ExecuteOrder(symbol, amount, BrokerOrderType.Sell);

        public Task<BrokerOrder> BuyAndWaitForResult(string symbol, decimal amount, int waitMilliseconds = 2000)
            => ExecuteOrderAndWaitForResult(symbol, amount, BrokerOrderType.Buy, waitMilliseconds);

        public Task<BrokerOrder> SellAndWaitForResult(string symbol, decimal amount, int waitMilliseconds = 2000)
            => ExecuteOrderAndWaitForResult(symbol, amount, BrokerOrderType.Sell, waitMilliseconds);

        private async Task<BrokerOrder> ExecuteOrderAndWaitForResult(
            string symbol,
            decimal amount,
            BrokerOrderType orderType,
            int waitMilliseconds)
        {
            if (orderType == BrokerOrderType.Sell)
            {
                var positions = await GetPositions();
                var stockAmount = positions.FirstOrDefault(x => x.Symbol == symbol)?.Amount ?? 0;
                if (stockAmount < amount)
                {
                    return new BrokerOrder
                    {
                        Status = BrokerOrderStatus.Failed,
                        FailedToSellDueToInsufficientAmountOfStocks = true
                    };
                }
            }

            await ExecuteOrder(symbol, amount, orderType);
            await Task.Delay(waitMilliseconds);
            var orders = await GetOrders();
            var order = orders.Where(x => x.Symbol == symbol && x.Type == orderType)
                .OrderByDescending(x => x.ExecutionTime)
                .FirstOrDefault();
            return order;
        }

        private async Task<string> ExecuteOrder(string symbol, decimal amount, BrokerOrderType type)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            await _orderService.ExecuteOrder(_accessToken, symbol, amount, type);
            return string.Empty;
        }

        private static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
            {
                url = newUrl;
            }

            return newUrl;
        }
    }
}
