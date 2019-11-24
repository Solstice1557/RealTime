namespace RealTime.BL
{
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using RestSharp;

    public static class RestSharpHelpers
    {
        public static Task<IRestResponse> GetResponseContentAsync(this RestClient client, RestRequest request)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, response => { tcs.SetResult(response); });
            return tcs.Task;
        }

        public static async Task<T> GetAndParse<T>(
            this RestClient client,
            RestRequest request,
            CancellationToken cancellationToken)
        {
            var response = await client.ExecuteTaskAsync(request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.Content);
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Empty response");
            }

            var data = JsonSerializer.Deserialize<T>(response.Content);
            if (data == null)
            {
                throw new Exception("Failed to parse response. Content: " + response.Content);
            }

            return data;
        }
    }
}
