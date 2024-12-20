using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.ETrade.Helpers;
using RealTime.BL.ETrade.Models;
using RealTime.BL.ETrade.Models.Response;
using RealTime.BL.Helpers;
using RealTime.BL.Brokers;

namespace RealTime.BL.ETrade
{
    public class ETradeAuthService : BaseETradeService
    {
        private const string AuthorizeLinkTemplate = "https://us.etrade.com/e/t/etws/authorize?key={0}&token={1}";
        private const string RequestTokenErrorMessage = "Could not get ETrade request token";
        private const string AccessTokenErrorMessage = "Could not get ETrade access token";
        private const string RefreshTokenErrorMessage = "Could not refresh ETrade access token";

        private const string ActiveAccountStatus = "active";
        private const string BrokerageAccountType = "brokerage";

        public ETradeAuthService(
            IHttpClientFactory httpClientFactory,
            ILogger<ETradeAuthService> logger,
            IOptions<ETradeConfig> configOptions) 
            : base(httpClientFactory, configOptions.Value, logger) { }

        public async Task<BrokerAuthPromptResponse> GetRequestToken()
        {
            const string requestPath = "oauth/request_token";

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey);

            var data = new ETradeRequest()
            {
                TokenSecret = string.Empty,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Get,
            };

            using var response = await GetETradeResponse(data, RequestTokenErrorMessage);
            var content = await response.Content.ReadAsStringAsync();

            var token = GetTokensFromResponse(content, out var tokenSecret);

            return new BrokerAuthPromptResponse()
            {
                LinkToken = string.Format(AuthorizeLinkTemplate, ETradeConfig.ConsumerKey, Uri.EscapeDataString(token)),
                OAuthToken = $"{token}|{tokenSecret}",
                RedirectLink = ETradeConfig.OAuthRedirectLink
            };
        }

        public async Task<BrokerAuthResponse> GetAccessToken(string token, string oauthVerifier)
        {
            if (string.IsNullOrEmpty(oauthVerifier))
            {
                return new BrokerAuthResponse()
                {
                    Status = BrokerAuthStatus.Failed,
                    ErrorMessage = "OAuth verification code is not provided"
                };
            }

            var tokenData = GetOAuthTokens(token, true);

            const string requestPath = "oauth/access_token";

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey);
            parameters.Add("oauth_verifier", oauthVerifier);
            parameters.Add("oauth_token", Uri.EscapeDataString(tokenData.OAuthToken));

            var data = new ETradeRequest()
            {
                TokenSecret = tokenData.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Get,
            };

            var response = await GetETradeResponse(data, AccessTokenErrorMessage);
            var content = await response.Content.ReadAsStringAsync();

            var userToken = GetTokensFromResponse(content, out var userTokenSecret);

            var accessToken = new ETradeOAuthToken()
            {
                OAuthToken = userToken,
                OAuthTokenSecret = userTokenSecret
            };

            var accounts = await GetAccounts(accessToken);

            if (accounts == null || !accounts.Any())
            {
                var errorMessage = "Could not find a brokerage account";
                Logger.LogTrace(errorMessage);

                return new BrokerAuthResponse
                {
                    Status = BrokerAuthStatus.Failed,
                    ErrorMessage = errorMessage
                };
            }

            return ConvertToBrokerAuthResponse(accounts, userToken, userTokenSecret);
        }

        public async Task<BrokerAuthResponse> RefreshAccessToken(string token)
        {
            const string requestPath = "oauth/renew_access_token";

            var tokenData = GetOAuthTokens(token);

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey);
            parameters.Add("oauth_token", Uri.EscapeDataString(tokenData.OAuthToken));

            var data = new ETradeRequest()
            {
                TokenSecret = tokenData.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Get,
            };

            await GetETradeResponse(data, RefreshTokenErrorMessage);

            return new BrokerAuthResponse()
            {
                Status = BrokerAuthStatus.Succeeded
            };
        }

        public async Task<BrokerAuthResponse> SwitchAccount(string decryptedToken)
        {
            var userTokens = GetOAuthTokens(decryptedToken);
            var accounts = await GetAccounts(userTokens);

            return ConvertToBrokerAuthResponse(accounts, userTokens.OAuthToken, userTokens.OAuthTokenSecret);
        }

        protected async Task<List<Account>> GetAccounts(ETradeOAuthToken token)
        {
            const string requestPath = "v1/accounts/list.json";

            var parameters = OAuth1Helper.GetBaseParameters(ETradeConfig.ConsumerKey, token.OAuthToken);

            var data = new ETradeRequest()
            {
                TokenSecret = token.OAuthTokenSecret,
                RequestPath = requestPath,
                Parameters = parameters,
                HttpMethod = HttpMethod.Get,
            };

            var response = await GetETradeResponse(data);
            var content = await response.Content.ReadAsStringAsync();

            var accounts = content.FromJson<EtradeAccounts>(JsonSerializerBehaviour.CaseInsensitive);

            return 
                accounts.AccountListResponse?.Accounts?.Account?.Where(x =>
                    x.AccountStatus.ToLower() == ActiveAccountStatus && x.InstitutionType.ToLower() == BrokerageAccountType).ToList();
        }

        private static BrokerAccount ConvertToBrokerAccount(Account account)
        {
            return new BrokerAccount()
            {
                AccountName = $"{account.AccountName} ({account.AccountId})",
                AccountId = account.AccountIdKey,
            };
        }

        private static BrokerAuthResponse ConvertToBrokerAuthResponse(
            IEnumerable<Account> accounts,
            string userToken,
            string userTokenSecret)
        {
            return new BrokerAuthResponse()
            {
                Status = BrokerAuthStatus.Succeeded,
                BrokerAccountTokens = accounts.Select(
                    x => new BrokerAccountTokens()
                    {
                        Account = ConvertToBrokerAccount(x),
                        AccessToken = $"{userToken}|{userTokenSecret}|{x.AccountIdKey}",
                        AccountName = x.AccountName
                    }).ToList()
            };
        }

        private static string GetTokensFromResponse(string content, out string tokenSecret)
        {
            var data = HttpUtility.ParseQueryString(content, Encoding.Default);

            var token = data["oauth_token"];
            tokenSecret = data["oauth_token_secret"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(tokenSecret))
            {
                throw new Exception($"{RequestTokenErrorMessage}: tokens are not provided");
            }

            return token;
        }
    }
}