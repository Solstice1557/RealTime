using AutoMapper;
using Microsoft.Extensions.Options;
using RealTime.BL.Brokers;
using RealTime.BL.Tdameritrade.Models;
using RealTime.BL.Tdameritrade.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RealTime.BL.Tdameritrade
{
    public class TdAmeritradeAuthService
    {
        private readonly IMapper mapper;
        private readonly ITdAmeritradeTokenService tdAmeritradeTokenService;
        private readonly ITdAmeritradeHttpClientService tdAmeritradeHttpClientService;
        private readonly TdAmeritradeConfig config;

        private const string OAuthAddress = "/v1/oauth2/token";
        private const string GetAccountsAddress = "/v1/accounts";


        public TdAmeritradeAuthService(
            IOptions<TdAmeritradeConfig> config,
            IMapper mapper,
            ITdAmeritradeTokenService tdAmeritradeTokenService,
            ITdAmeritradeHttpClientService tdAmeritradeHttpClientService)
        {
            this.config = config.Value;
            this.mapper = mapper;
            this.tdAmeritradeTokenService = tdAmeritradeTokenService;
            this.tdAmeritradeHttpClientService = tdAmeritradeHttpClientService;
        }

        public async Task<BrokerAuthResponse> Authenticate(string authorizationCode)
        {
            // docs in https://developer.tdameritrade.com/authentication/apis/post/token-0

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, OAuthAddress);

            var formDict = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["access_type"] = "offline",
                ["code"] = authorizationCode,
                ["client_id"] = config.ClientId,
                ["redirect_uri"] = config.RedirectUrl,
            };

            httpRequest.Content = new FormUrlEncodedContent(formDict);

            return await EnrichWithAccountTokens(
                mapper.Map<BrokerAuthResponse>(
                    await tdAmeritradeHttpClientService.Execute<AccessTokenResponse>(httpRequest)));
        }

        public async Task<BrokerAuthResponse> RefreshRefreshToken(string refreshToken)
        {
            (var accountId, var token) = tdAmeritradeTokenService.Split(refreshToken);

            // docs in https://developer.tdameritrade.com/authentication/apis/post/token-0
            // and in https://developer.tdameritrade.com/content/authentication-faq

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, OAuthAddress);

            var formDict = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["access_type"] = "offline",
                ["refresh_token"] = token,
                ["client_id"] = config.ClientId,
            };

            httpRequest.Content = new FormUrlEncodedContent(formDict);

            var newToken = mapper.Map<BrokerAuthResponse>(
                    await tdAmeritradeHttpClientService.Execute<AccessTokenResponse>(httpRequest));

            newToken.RefreshToken = string.IsNullOrWhiteSpace(newToken.RefreshToken) 
                ? null : tdAmeritradeTokenService.Concatenate(accountId, newToken.RefreshToken);
            newToken.AccessToken = string.IsNullOrWhiteSpace(newToken.AccessToken)
                ? null : tdAmeritradeTokenService.Concatenate(accountId, newToken.AccessToken);

            return newToken;
        }

        public async Task<BrokerAuthResponse> RefreshAccessToken(string refreshToken)
        {
            (var accountId, var token) = tdAmeritradeTokenService.Split(refreshToken);

            // docs in https://developer.tdameritrade.com/authentication/apis/post/token-0
            // and in https://developer.tdameritrade.com/content/authentication-faq

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, OAuthAddress);

            var formDict = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = token,
                ["client_id"] = config.ClientId,
            };

            httpRequest.Content = new FormUrlEncodedContent(formDict);

            var newToken = mapper.Map<BrokerAuthResponse>(
                    await tdAmeritradeHttpClientService.Execute<AccessTokenResponse>(httpRequest));

            newToken.RefreshToken = string.IsNullOrWhiteSpace(newToken.RefreshToken)
                ? null : tdAmeritradeTokenService.Concatenate(accountId, newToken.RefreshToken);
            newToken.AccessToken = string.IsNullOrWhiteSpace(newToken.AccessToken)
                ? null : tdAmeritradeTokenService.Concatenate(accountId, newToken.AccessToken);

            return newToken;
        }

        public async Task<BrokerAuthResponse> SwitchAccount(string refreshToken)
        {
            var newToken = await RefreshRefreshToken(refreshToken);

            if (!string.IsNullOrWhiteSpace(newToken.RefreshToken))
            {
                (_, newToken.RefreshToken) = tdAmeritradeTokenService.Split(newToken.RefreshToken);
            }
            if (!string.IsNullOrWhiteSpace(newToken.AccessToken))
            {
                (_, newToken.AccessToken) = tdAmeritradeTokenService.Split(newToken.AccessToken);
            }

            return await EnrichWithAccountTokens(newToken);
        }

        public  BrokerAuthPromptResponse GetProviderAuthLink()
        {
            return new BrokerAuthPromptResponse
            {
                Status = BrokerAuthPromptStatus.Redirect,
                LinkToken = config.TdAmeritradeAuthWebAddress,
                RedirectLink = config.RedirectUrl
            };
        }

        private async Task<BrokerAuthResponse> EnrichWithAccountTokens(BrokerAuthResponse token)
        {
            var accounts = await GetAccounts(token.AccessToken);

            token.BrokerAccountTokens = accounts.AccountList.Select(x => new BrokerAccountTokens
            {
                Account = x,
                AccessToken  = string.IsNullOrWhiteSpace(token.AccessToken)  
                ? null : tdAmeritradeTokenService.Concatenate(x.AccountId,token.AccessToken),
                RefreshToken = string.IsNullOrWhiteSpace(token.RefreshToken) 
                ? null : tdAmeritradeTokenService.Concatenate(x.AccountId, token.RefreshToken)
            }).ToList();

            token.AccessToken = null;
            token.RefreshToken = null;

            return token;
        }

        private async Task<BrokerAccountList> GetAccounts(
            string accessToken)
        {
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                GetAccountsAddress);

            return new BrokerAccountList
            {
                AccountList =
                (await tdAmeritradeHttpClientService.Execute<GetAccountResponse[]>(
                httpRequest,
                accessToken))
                .Select(x => mapper.Map<BrokerAccount>(x.SecuritiesAccount))
                .ToList()
            };
        }
    }
}
