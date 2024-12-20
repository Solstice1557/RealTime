using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RealTime.BL.ETrade.Helpers;
using RealTime.BL.ETrade.Models;
using RealTime.BL.ETrade.Models.Response;
using RealTime.BL.Helpers;

namespace RealTime.BL.ETrade
{
    public class BaseETradeService
    {
        private readonly IHttpClientFactory httpClientFactory;
        protected readonly ETradeConfig ETradeConfig;
        protected readonly ILogger Logger;

        protected const string EquityProductType = "EQ";

        public BaseETradeService(
            IHttpClientFactory httpClientFactory,
            ETradeConfig eTradeConfig,
            ILogger logger)
        {
            this.httpClientFactory = httpClientFactory;
            ETradeConfig = eTradeConfig;
            Logger = logger;
        }

        protected HttpClient CreateHttpClient() => httpClientFactory.CreateClient("etrade");

        internal async Task<HttpResponseMessage> GetETradeResponse(
            ETradeRequest eTradeRequest,
            string errorMessage = null)
        {
            if (eTradeRequest.QueryParameters != null)
            {
                foreach (var (key, value) in eTradeRequest.QueryParameters)
                {
                    eTradeRequest.Parameters.Add(key, value);
                }
            }

            var url = $"{ETradeConfig.Endpoint}{eTradeRequest.RequestPath}";
            var signature = OAuth1Helper.GetHmacSignature(url, eTradeRequest.Parameters, ETradeConfig.ConsumerSecret,
                Uri.EscapeDataString(eTradeRequest.TokenSecret),
                eTradeRequest.HttpMethod ?? HttpMethod.Get);

            var header = OAuth1Helper.CreateAuthorizationHeader(eTradeRequest.Parameters, signature);

            if (eTradeRequest.QueryParameters != null && eTradeRequest.QueryParameters.Any())
            {
                var sb = new StringBuilder();
                sb.Append("?");
                var (key, value) = eTradeRequest.QueryParameters.FirstOrDefault();
                sb.Append(key).Append("=").Append(value);

                foreach (var parameter in eTradeRequest.QueryParameters.Skip(1))
                {
                    sb.Append("&");
                    sb.Append(parameter.Key);
                    sb.Append("=");
                    sb.Append(parameter.Value);
                }

                url += sb;
            }

            using var request = new HttpRequestMessage(eTradeRequest.HttpMethod ?? HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(eTradeRequest.Content))
            {
                request.Content = new StringContent(eTradeRequest.Content, Encoding.UTF8, "application/xml");
            }

            request.Headers.Add("Authorization", header);
            using var client = CreateHttpClient();
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var authHeader = response.Headers.WwwAuthenticate.FirstOrDefault();
                    if (authHeader != null && authHeader.Parameter.Contains("token_expired"))
                    {
                        const string shouldBeRefreshedMessage = "ETrade token should be refreshed.";
                        Logger.LogTrace(shouldBeRefreshedMessage);
                        throw new Exception(shouldBeRefreshedMessage);
                    }

                    var unauthorizedMessage = $"Unauthorized ETrade call: {response.ReasonPhrase}";
                    Logger.LogError($"{unauthorizedMessage} Content: {content}");
                    throw new Exception(unauthorizedMessage);
                }

                var parsedError = content.TryFromJson<ErrorResponse>(JsonSerializerBehaviour.CaseInsensitive);
                if (parsedError?.Error != null)
                {
                    if (parsedError.Error.Code == 101 && parsedError.Error.Message.Contains("please resubmit it now"))
                    {
                        return await GetETradeResponse(eTradeRequest, errorMessage);
                    }
                }

                var message = errorMessage ?? "Error making ETrade API request call";

                var parsedMessage = parsedError?.Error?.Message;
                if (!string.IsNullOrEmpty(parsedMessage))
                {
                    message += $": {parsedMessage}";
                }

                Logger.LogError($"{message} Content: {content}");
                throw new Exception($"{message}: {response.ReasonPhrase}.");
            }

            return response;
        }

        protected ETradeOAuthToken GetOAuthTokens(string decryptedToken, bool isRequestToken = false)
        {
            const string separator = "|";
            const string errorMessage = "Token data corrupted.";

            if (!decryptedToken.Contains(separator))
            {
                throw new Exception(errorMessage);
            }

            var tokens = decryptedToken.Split(separator);

            if (!isRequestToken && tokens.Length != 3 || isRequestToken && tokens.Length != 2)
            {
                throw new Exception(errorMessage);
            }

            return new ETradeOAuthToken
            {
                OAuthToken = tokens[0],
                OAuthTokenSecret = tokens[1],
                AccountId = isRequestToken ? null : tokens[2]
            };
        }

        protected string GetUniqueETradeId()
        {
            return OAuth1Helper.GetNonce(8);
        }
    }
}
