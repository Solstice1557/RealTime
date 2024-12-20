using RealTime.BL.Brokers;
using RealTime.BL.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace RealTime.BL.Tdameritrade
{
    public class TdAmeritradeFacade
    {
        private const string TokenFileName = "TdAmeritradeRefreshToken.txt";

        private readonly TdAmeritradeAuthService _authService;
        private readonly TdAmeritradePortfolioService _portfolioService;
        private string _accessToken;
        private string _refreshToken;

        private Timer _refreshTokenTimer;

        public TdAmeritradeFacade(
            TdAmeritradeAuthService authService,
            TdAmeritradePortfolioService portfolioService)
        {
            _authService = authService;
            _portfolioService = portfolioService;
        }

        public async Task<bool> Authenticate()
        {
            if (File.Exists(TokenFileName))
            {
                _refreshToken = File.ReadAllText(TokenFileName);

                try
                {
                    var refreshResult = RefreshToken();
                    if (refreshResult)
                    {
                        return true;
                    }
                }
                catch
                {
                }
            }

            var authLink = _authService.GetProviderAuthLink();

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

            var accounts = authResponse.BrokerAccountTokens.ToArray();
            _accessToken = accounts[0].AccessToken;
            _refreshToken = accounts[0].RefreshToken;
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

                _accessToken = accounts[index - 1].AccessToken;
                _refreshToken = accounts[index - 1].RefreshToken;
            }

            SaveRefreshToken();
            StartRefreshTokenTimer(authResponse.ExpiresInSeconds ?? 1800);

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

            var ordersList = await _portfolioService.GetOrders(_accessToken);
            return ordersList.OrderList;
        }

        public async Task CancelOrder(string id)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new InvalidOperationException("Authenticate first");
            }

            await _portfolioService.CancelOrder(_accessToken, id);
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

            await _portfolioService.ExecuteOrder(_accessToken, symbol, amount, type);
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

        private void StartRefreshTokenTimer(int expiresInSeconds)
        {
            if (_refreshTokenTimer != null)
            {
                _refreshTokenTimer.Stop();
                _refreshTokenTimer.Dispose();
            }

            var timerInMilliseconds = (expiresInSeconds - 60) * 1000;

            _refreshTokenTimer = new Timer(timerInMilliseconds);
            _refreshTokenTimer.Elapsed += (s, e) => RefreshToken();

            _refreshTokenTimer.Start();
        }

        private bool RefreshToken()
        {
            var refreshResult = _authService.RefreshAccessToken(_refreshToken).Result;
            if (refreshResult.Status != BrokerAuthStatus.Succeeded)
            {
                return false;
            }

            _accessToken = refreshResult.AccessToken;
            if (!string.IsNullOrEmpty(refreshResult.RefreshToken) && _refreshToken != refreshResult.RefreshToken)
            {
                _refreshToken = refreshResult.RefreshToken;
                SaveRefreshToken();
            }

            StartRefreshTokenTimer(refreshResult.ExpiresInSeconds ?? 1800);
            return true;
        }

        private void SaveRefreshToken()
        {
            File.WriteAllText(TokenFileName, _refreshToken);
        }
    }
}
