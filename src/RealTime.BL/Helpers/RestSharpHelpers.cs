﻿namespace RealTime.BL
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using RestSharp;

    public static class RestSharpHelpers
    {
        public static async Task<T> GetAndParse<T>(
            this RestClient client,
            RestRequest request,
            CancellationToken cancellationToken)
        {
            var response = await client.ExecuteAsync(request, cancellationToken);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.Content);
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                throw new Exception("Empty response");
            }

            var data = JsonConvert.DeserializeObject<T>(response.Content);
            if (data == null)
            {
                throw new Exception("Failed to parse response. Content: " + response.Content);
            }

            return data;
        }
    }
}
