using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Alpaca.Models;
using RealTime.BL.Brokers;
using RealTime.BL.Helpers;

namespace RealTime.BL.Alpaca
{
    public class AlpacaAuthService : BaseAlpacaMarketService, IAlpacaAuthService
    {
        private static readonly string authorizeLinkTemplate =
            "https://app.alpaca.markets/oauth/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope=account:write%20trading";

        private const string GetAccessTokenEndpoint = "https://api.alpaca.markets/oauth/token";

        private readonly HttpClient httpClient;
        private readonly ILogger<AlpacaAuthService> logger;

        public AlpacaAuthService(
            IOptions<AlpacaConfig> config,
            ILogger<AlpacaAuthService> logger,
            HttpClient httpClient)
            : base(config, logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public BrokerAuthPromptResponse GetLinkToken()
        {
            var link = string.Format(authorizeLinkTemplate, AlpacaConfig.AlpacaClientId, AlpacaConfig.AlpacaOAuthRedirectLink);
            return new BrokerAuthPromptResponse()
            {
                Status = BrokerAuthPromptStatus.Redirect,
                LinkToken = link,
                RedirectLink = AlpacaConfig.AlpacaOAuthRedirectLink
            };
        }

        public async Task<BrokerAuthResponse> Authenticate(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetAccessTokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", token),
                    new KeyValuePair<string, string>("client_id", AlpacaConfig.AlpacaClientId),
                    new KeyValuePair<string, string>("client_secret", AlpacaConfig.AlpacaClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", AlpacaConfig.AlpacaOAuthRedirectLink),
                })
            };
            
            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorModel = content.TryFromJson<ErrorModel>();
                throw BrokerErrorResponseHelper.CreateBrokerException(
                    response.StatusCode,
                    errorModel?.Message,
                    content,
                    "Alpaca",
                    logger);
            }

            var result = content.FromJson<AuthResponse>();

            var account = await GetAccount(result.AccessToken);
            
            var tokenModel = new TokenModel
            {
                AccountId = account.AccountId.ToString(),
                Token = result.AccessToken
            };

            return new BrokerAuthResponse
            {
                AccessToken = CreateTokenWithAccountId(tokenModel),
                Status = BrokerAuthStatus.Succeeded,
            };
        }

        public string GetAccountId(string decryptedToken)
        {
            return decryptedToken.Contains("|") ? decryptedToken.Split("|")[1] : null;
        }
    }
}
