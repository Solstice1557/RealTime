using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.ETrade.Helpers;
using RealTime.BL.ETrade.Models;
using System.Collections.Generic;
using System.Linq;
using RealTime.BL.ETrade.Models.Response;
using RealTime.BL.Helpers;
using System;
using RealTime.BL.Brokers;

namespace RealTime.BL.ETrade
{
    public class ETradePortfolioService : BaseETradeService
    {
        private const string MarginAccountType = "margin";

        public ETradePortfolioService(
            IHttpClientFactory httpClientFactory,
            ILogger<ETradePortfolioService> logger,
            IOptions<ETradeConfig> configOptions)
            : base(httpClientFactory, configOptions.Value, logger) { }

        public async Task<List<BrokerPosition>> GetPositions(string token)
        {
            var tokenData = GetOAuthTokens(token);
            
            var portfolio = await GetPortfolio(tokenData.AccountId, tokenData.OAuthToken, tokenData.OAuthTokenSecret);

            if (portfolio == null)
            {
                return new List<BrokerPosition>(0);
            }

            return portfolio.PortfolioResponse.AccountPortfolio.SelectMany(x => x.Position)
                .Where(x => x.Product.SecurityType == EquityProductType)
                .Select(
                    position => new BrokerPosition
                    {
                        Amount = position.Quantity,
                        Symbol = position.Product.Symbol,
                        CostBasis = position.PricePaid
                    })
                .ToList();
        }

        public async Task<BrokerBalance> GetBalance(string decryptedToken)
        {
            var tokenData = GetOAuthTokens(decryptedToken);
            var requestPath = $"v1/accounts/{tokenData.AccountId}/balance.json";

            var queryParameters = new Dictionary<string, string>() {
                {"instType", "BROKERAGE" },
                {"realTimeNAV", "true" }
            };

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, tokenData.OAuthToken);

            var data = new ETradeRequest()
            {
                TokenSecret = tokenData.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Get,
                QueryParameters = queryParameters
            };

            var response = await GetETradeResponse(data);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                const string errorMessage = "Could not get ETrade account balance";
                Logger.LogError($"{errorMessage}: {content}");
                throw new Exception($"{errorMessage}: {response.ReasonPhrase}");
            }

            var balance = content.FromJson<BalanceResponseModel>(JsonSerializerBehaviour.CaseInsensitive);
            return new BrokerBalance
            {
                Cash = balance.BalanceResponse.Computed.CashAvailableForInvestment,
                BuyingPower = balance.BalanceResponse.AccountType.ToLower() == MarginAccountType
                    ? balance.BalanceResponse.Computed.MarginBuyingPower
                    : balance.BalanceResponse.Computed.CashBuyingPower,
                Equity = balance.BalanceResponse.Computed.RealTimeValues.TotalAccountValue
                    - balance.BalanceResponse.Computed.CashAvailableForInvestment
            };
        }

        private async Task<PortfolioDto> GetPortfolio(string accountId, string token, string tokenSecret)
        {
            var requestPath = $"v1/accounts/{accountId}/portfolio.json";

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, token);

            var data = new ETradeRequest()
            {
                TokenSecret = tokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Get,
            };

            var response = await GetETradeResponse(data);
            
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                const string errorMessage = "Could not get ETrade portfolio";
                Logger.LogError($"{errorMessage}: {content}");
                throw new Exception($"{errorMessage}: {response.ReasonPhrase}");
            }

            var portfolioDto = content.FromJson<PortfolioDto>(JsonSerializerBehaviour.CaseInsensitive);
            return portfolioDto;
        }
    }
}