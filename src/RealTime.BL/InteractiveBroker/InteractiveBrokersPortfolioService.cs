using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Brokers;
using RealTime.BL.Encryption;
using RealTime.BL.InteractiveBroker.Models;
using RealTime.BL.InteractiveBroker.Models.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace RealTime.BL.InteractiveBroker
{
    public class InteractiveBrokersPortfolioService : BaseInteractiveBrokersService
    {
        public InteractiveBrokersPortfolioService(
            IOptions<InteractiveBrokersConfig> configOptions,
            HttpClient httpClient,
            ILogger<InteractiveBrokersPortfolioService> logger,
            IKeyVaultService keyVaultService)
            : base(configOptions.Value, httpClient, logger, keyVaultService)
        {
        }

        public async Task<IReadOnlyCollection<BrokerPosition>> GetPositions(string decryptedToken)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/positions";

            var headerPos = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Get);
            var accountsPositions = await CreateAndExecuteHttpRequest<List<AccountPositionResponse>>(
                headerPos,
                url,
                BrokerOperationNames.GetPortfolio,
                HttpMethod.Get);

            var result = new List<BrokerPosition>();
            foreach (var position in accountsPositions)
            {
                var security = await GetSecurityDefinition(position.ContractId, tokenModel);
                result.Add(new BrokerPosition()
                {
                    Amount = Convert.ToDecimal(position.Position, CultureInfo.InvariantCulture),
                    CostBasis = Convert.ToDecimal(position.AverageCost, CultureInfo.InvariantCulture),
                    Symbol = security.Ticker
                });
            }

            return result;
        }

        private async Task<SecurityResponse> GetSecurityDefinition(
            string connectionId,
            InteractiveBrokersOAuthTokenModel model)
        {
            var url = $"{InteractiveBrokersConfig.Endpoint}secdef";
            var secDefUrl = $"{url}?conid={connectionId}";

            var headerSecDef = CreateAuthHeaderWithLiveSessionSignature(
                model.OAuthToken,
                model.LiveSessionToken,
                url,
                HttpMethod.Get,
                false,
                new Dictionary<string, string>()
            {
                {"conid", connectionId}
            });

            return await CreateAndExecuteHttpRequest<SecurityResponse>(
                headerSecDef,
                secDefUrl,
                BrokerOperationNames.GetSymbolInfo,
                HttpMethod.Get);
        }

        public async Task<BrokerBalance> GetBalance(string decryptedToken)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts/{tokenModel.AccountId}/summary";

            var headerPos = CreateAuthHeaderWithLiveSessionSignature(
                tokenModel.OAuthToken,
                tokenModel.LiveSessionToken,
                url,
                HttpMethod.Get);
            var accountSummary = await CreateAndExecuteHttpRequest<AccountSummary>(
                headerPos,
                url,
                BrokerOperationNames.GetBalance,
                HttpMethod.Get);

            return new BrokerBalance
            {
                BuyingPower = accountSummary.Summary.USD.BuyingPower,
                Cash = accountSummary.Summary.USD.TotalCashValue,
            };
        }
    }
}
