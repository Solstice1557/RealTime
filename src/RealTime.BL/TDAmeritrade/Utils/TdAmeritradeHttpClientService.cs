using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RealTime.BL.Tdameritrade.Utils
{
    public class TdAmeritradeHttpClientService : ITdAmeritradeHttpClientService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger logger;
        
        public TdAmeritradeHttpClientService(
            IHttpClientFactory httpClientFactory,
            ILogger<TdAmeritradeHttpClientService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<T> Execute<T>(
            HttpRequestMessage httpRequest,
            string accessToken = null)
            where T : class
        {
            var content = await Execute(httpRequest, accessToken);

            if (string.IsNullOrWhiteSpace(content))
            {
                logger.LogError("Empty response received.");
                return null;
            }

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());
            options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<T>(content, options);
        }

        public async Task<string> Execute(
            HttpRequestMessage httpRequest,
            string accessToken = null)
        {
            if (accessToken != null)
            {
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            using var httpClient = httpClientFactory.CreateClient(Constant.TdAmeritradeHttpClientName);
            using var response = await httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception($"Unauthorized api call. Status: {response.StatusCode} Response: {content}");
                }

                throw new Exception($"Failed to execute request. Status: {response.StatusCode} Response: {content}");
            }

            return content;
        }
    }
}
