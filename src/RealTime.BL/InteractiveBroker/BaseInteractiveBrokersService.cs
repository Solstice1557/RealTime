using Microsoft.Extensions.Logging;
using RealTime.BL.Encryption;
using RealTime.BL.Helpers;
using RealTime.BL.InteractiveBroker.Models;
using RealTime.BL.InteractiveBroker.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RealTime.BL.InteractiveBroker
{
    public class BaseInteractiveBrokersService
    {
        protected readonly HttpClient HttpClient;
        protected readonly InteractiveBrokersConfig InteractiveBrokersConfig;
        protected readonly ILogger Logger;

        public BaseInteractiveBrokersService(
            InteractiveBrokersConfig interactiveBrokersConfig,
            HttpClient httpClient,
            ILogger logger,
            IKeyVaultService keyVaultService)
        {
            InteractiveBrokersConfig = interactiveBrokersConfig;
            HttpClient = httpClient;
            Logger = logger;
            KeyVaultService = keyVaultService;
        }

        protected IKeyVaultService KeyVaultService { get; }

        protected async Task<string> RsaSignSignatureWithPrivateKey(
            string url,
            SortedDictionary<string, string> parameters,
            HttpMethod httpMethod)
        {
            var signatureBase = OAuth1Helper.GetSignatureBase(httpMethod, url, parameters);
            var signatureBaseBytes = Encoding.UTF8.GetBytes(signatureBase);
            return await SignRSA(signatureBaseBytes);
        }

        protected async Task<string> SignRSA(byte[] signatureBaseBytes)
        {
            var signature =
                await KeyVaultService.SignRSA(signatureBaseBytes, InteractiveBrokersConfig.KeyVaultSignatureKeyName);
            var signatureBase64 = Convert.ToBase64String(signature);
            return Uri.EscapeDataString(signatureBase64);
        }

        public async Task<AccountsModelResponse> GetAccounts(string accessToken, string liveSessionToken)
        {
            var url = $"{InteractiveBrokersConfig.Endpoint}accounts";

            var header = CreateAuthHeaderWithLiveSessionSignature(accessToken, liveSessionToken, url, HttpMethod.Get);
            return await CreateAndExecuteHttpRequest<AccountsModelResponse>(
                header,
                url,
                BrokerOperationNames.GetAccounts,
                HttpMethod.Get);
        }

        protected async Task<T> CreateAndExecuteHttpRequest<T>(
            string header,
            string url,
            string operationName,
            HttpMethod httpMethod,
            bool isOauthRequest = false,
            JsonSerializerBehaviour serializerBehaviour = JsonSerializerBehaviour.Default,
            Dictionary<string, string> formModel = null) where T : class
        {
            var request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Add("authorization", header);

            //Not working without this:
            request.Headers.Add("User-Agent", "Asp.net");

            if (formModel != null)
            {
                request.Content = new FormUrlEncodedContent(formModel.ToList());
            }

            var response = await HttpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // If account was created the same day, IB may return HTTP status 204 (No Content)
                // and account becomes accessible the next day only (after their nightly refresh)
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    var errorMessage = "Empty response from Interactive Brokers";
                    Logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                //Different responses return differently serialized JSON
                var behaviour = serializerBehaviour == JsonSerializerBehaviour.Default
                    ? isOauthRequest
                        ? JsonSerializerBehaviour.SnakeCase
                        : JsonSerializerBehaviour.CaseInsensitive
                    : serializerBehaviour;

                var result = content.FromJson<T>(behaviour);
                return result;
            }

            var parsedError = content.TryFromJson<ErrorResponse>(JsonSerializerBehaviour.CaseInsensitive);
            var parsedMessage = parsedError?.Error;

            var msg = $"Interactive Brokers error, {response.StatusCode}, {parsedMessage}, {content}, {operationName}";
            throw new Exception(msg);
        }

        protected string CreateAuthHeaderWithLiveSessionSignature(
            string accessToken,
            string liveSessionToken,
            string url,
            HttpMethod method,
            bool addRealm = false,
            Dictionary<string, string> additionalParameters = null)
        {
            var parameters = OAuth1Helper.GetBaseParameters(InteractiveBrokersConfig.ConsumerKey, accessToken);

            if (additionalParameters != null)
            {
                foreach (var (key, value) in additionalParameters)
                {
                    parameters.Add(key, value);
                }
            }

            var signatureBase = OAuth1Helper.GetSignatureBase(method, url, parameters);
            var signatureBaseBytes = Encoding.UTF8.GetBytes(signatureBase);
            var signature = LiveSessionTokenHelper.SignWithLiveTokenHmacSha256(
                Convert.FromBase64String(liveSessionToken),
                signatureBaseBytes);

            if (addRealm)
            {
                parameters.Add("realm", "limited_poa");
            }

            var header = OAuth1Helper.CreateAuthorizationHeader(parameters, signature);
            return header;
        }

        protected InteractiveBrokersOAuthTokenModel GetTokenModel(string decryptedToken)
        {
            const string separator = "|";
            const string errorMessage = "Token data corrupted.";

            if (!decryptedToken.Contains(separator))
            {
                throw new Exception(errorMessage);
            }

            var tokens = decryptedToken.Split(separator);

            if (tokens.Length != 4)
            {
                throw new Exception(errorMessage);
            }

            return new InteractiveBrokersOAuthTokenModel()
            {
                AccountId = tokens[0],
                OAuthToken = tokens[1],
                OAuthTokenSecret = tokens[2],
                LiveSessionToken = tokens[3]
            };
        }
    }
}
