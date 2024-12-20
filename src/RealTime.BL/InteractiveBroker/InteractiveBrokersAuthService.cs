using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Brokers;
using RealTime.BL.Encryption;
using RealTime.BL.InteractiveBroker.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RealTime.BL.InteractiveBroker
{
    public class InteractiveBrokersAuthService : BaseInteractiveBrokersService
    {
        public InteractiveBrokersAuthService(
            IOptions<InteractiveBrokersConfig> configOptions,
            HttpClient httpClient,
            ILogger<InteractiveBrokersAuthService> logger,
            IKeyVaultService keyVaultService) 
            : base(configOptions.Value, httpClient, logger, keyVaultService)
        {
        }

        public async Task<BrokerAuthPromptResponse> GetRequestToken()
        {
            var url = $"{InteractiveBrokersConfig.Endpoint}oauth/request_token";

            var parameters = OAuth1Helper.GetBaseParameters(
                InteractiveBrokersConfig.ConsumerKey,
                null,
                true,
                false,
                "RSA-SHA256");

            var signature = await RsaSignSignatureWithPrivateKey(url, parameters, HttpMethod.Post);

            parameters.Add("realm", "limited_poa");

            var header = OAuth1Helper.CreateAuthorizationHeader(parameters, signature);
            var requestToken = await CreateAndExecuteHttpRequest<RequestTokenResponse>(
                header,
                url,
                BrokerOperationNames.Authenticate,
                HttpMethod.Post,
                true);

            return new BrokerAuthPromptResponse()
            {
                LinkToken = string.Format(InteractiveBrokersConfig.OAuthAuthLinkTemplate, requestToken.OauthToken),
                OAuthToken = requestToken.OauthToken,
                RedirectLink = InteractiveBrokersConfig.OAuthRedirectLink
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

            var url = $"{InteractiveBrokersConfig.Endpoint}oauth/access_token";

            var parameters = OAuth1Helper.GetBaseParameters(
                InteractiveBrokersConfig.ConsumerKey,
                null,
                true,
                false,
                "RSA-SHA256");

            parameters.Add("oauth_token", token);
            parameters.Add("oauth_verifier", oauthVerifier);

            var signature = await RsaSignSignatureWithPrivateKey(url, parameters, HttpMethod.Post);

            //Some oauth methods require this, some not:
            //Added after other parameters because it's not a part of the signature
            parameters.Add("realm", "limited_poa");

            var header = OAuth1Helper.CreateAuthorizationHeader(parameters, signature);
            var accessTokenModel = await CreateAndExecuteHttpRequest<AccessTokenResponse>(
                header,
                url,
                BrokerOperationNames.Authenticate,
                HttpMethod.Post,
                true);

            var encryptedBytes = Convert.FromBase64String(accessTokenModel.OauthTokenSecret);
            var decryptedAccessTokenSecret = await KeyVaultService.DecryptRSA(encryptedBytes,
                InteractiveBrokersConfig.KeyVaultEncryptionKeyName);

            var decryptedAccessTokenSecretBase64 = Convert.ToBase64String(decryptedAccessTokenSecret);

            return await GetLiveSessionTokenAndAccounts(
                accessTokenModel.OauthToken,
                decryptedAccessTokenSecretBase64);
        }


        public string GetAccountId(string decryptedToken)
        {
            var token = GetTokenModel(decryptedToken);
            return token.AccountId;
        }

        public async Task<BrokerAuthResponse> RefreshAccessToken(string decryptedToken)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            return await GetLiveSessionTokenAndAccounts(tokenModel.OAuthToken, tokenModel.OAuthTokenSecret);
        }

        public async Task<BrokerAuthResponse> SwitchAccount(string decryptedToken)
        {
            var tokenModel = GetTokenModel(decryptedToken);
            var accounts = await GetAccounts(tokenModel.OAuthToken, tokenModel.LiveSessionToken);
            return ConvertToBrokerAuthResponse(accounts.Accounts, tokenModel.OAuthToken,
                tokenModel.OAuthTokenSecret, tokenModel.LiveSessionToken);
        }

        private async Task<BrokerAuthResponse> GetLiveSessionTokenAndAccounts(
            string oAuthToken,
            string decryptedAccessTokenSecretBase64)
        {
            var liveSessionToken = await GetLiveSessionToken(oAuthToken, decryptedAccessTokenSecretBase64);

            var accounts = await GetAccounts(oAuthToken, liveSessionToken);
            return ConvertToBrokerAuthResponse(
                accounts.Accounts,
                oAuthToken,
                decryptedAccessTokenSecretBase64,
                liveSessionToken);
        }

        private static BrokerAuthResponse ConvertToBrokerAuthResponse(IEnumerable<string> accounts, string oauthToken, string oauthTokenSecret, string liveSessionToken)
        {
            var dayInSeconds = Convert.ToInt32(TimeSpan.FromDays(1).TotalSeconds);
            return new BrokerAuthResponse()
            {
                Status = BrokerAuthStatus.Succeeded,
                ExpiresInSeconds = dayInSeconds,
                BrokerAccountTokens = accounts.Select(x =>
                {
                    var token = $"{x}|{oauthToken}|{oauthTokenSecret}|{liveSessionToken}";
                    return new BrokerAccountTokens()
                    {
                        Account = new BrokerAccount()
                        {
                            AccountId = x,
                            AccountName = x
                        },
                        AccessToken = token,
                        RefreshToken = token
                    };
                }).ToList()
            };
        }


        // This step, which is not defined in oauth, establishes a shared secret that will be used to
        // sign requests to access protected resources.
        // Documentation can be found here: https://www1.interactivebrokers.com/webtradingapi/oauth.pdf
        private async Task<string> GetLiveSessionToken(string accessToken, string oauthSecretDecrypted)
        {
            var url = $"{InteractiveBrokersConfig.Endpoint}oauth/live_session_token";

            var parameters = OAuth1Helper.GetBaseParameters(
                InteractiveBrokersConfig.ConsumerKey,
                null,
                false,
                false,
                "RSA-SHA256");

            var prime = LiveSessionTokenHelper.ParseHexStringToBigInteger(
                InteractiveBrokersConfig.DiffieHellmanPrimeSecret);
            var generator = 2;

            var randomExponent = DiffieHellmanHelper.GetRandomBytes();

            var dh = DiffieHellmanHelper.GenerateDiffieHellman(randomExponent, generator, prime);
            var diffieHellmanChallenge = dh.ToString("X");

            parameters.Add("diffie_hellman_challenge", diffieHellmanChallenge);
            parameters.Add("oauth_token", accessToken);

            var signatureBase = OAuth1Helper.GetSignatureBase(HttpMethod.Post, url, parameters);
            var oauthSecretHex = DiffieHellmanHelper.GetBase64InHex(oauthSecretDecrypted);
            signatureBase = oauthSecretHex + signatureBase;

            var signatureBaseBytes = Encoding.UTF8.GetBytes(signatureBase);

            var signature = await SignRSA(signatureBaseBytes);

            parameters.Add("realm", "limited_poa");

            var header = OAuth1Helper.CreateAuthorizationHeader(parameters, signature);
            var liveTokenModel = await CreateAndExecuteHttpRequest<LiveSessionTokenResponse>(
                header,
                url,
                BrokerOperationNames.Authenticate,
                HttpMethod.Post,
                true);

            var hexExponent = randomExponent.ToString("X");
            return LiveSessionTokenHelper.ComputeLiveSessionToken(
                hexExponent,
                oauthSecretDecrypted,
                liveTokenModel.DiffieHellmanResponse,
                InteractiveBrokersConfig.DiffieHellmanPrimeSecret);
        }
    }
}
