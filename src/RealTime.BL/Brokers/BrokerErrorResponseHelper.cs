using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace RealTime.BL.Brokers
{
    public static class BrokerErrorResponseHelper
    {
        public static Exception CreateBrokerException(
            HttpStatusCode statusCode,
            string parsedMessage,
            string content,
            string brokerName,
            ILogger logger)
        {
            if (!string.IsNullOrEmpty(parsedMessage))
            {
                parsedMessage = parsedMessage.Replace("_", " ");
            }

            //Make sure all erroneous responses are logged including content:
            logger.LogError(
                "Could not execute broker request. Broker name: {BrokerName}. Status = {StatusCode}. Content = {Content}",
                brokerName,
                statusCode,
                content);

            return statusCode switch
            {
                HttpStatusCode.Unauthorized => new Exception(
                    $"Unauthorized api call. Status: {statusCode}. Error message: {parsedMessage ?? content}"),
                HttpStatusCode.BadRequest when !string.IsNullOrEmpty(parsedMessage) =>
                    new Exception(parsedMessage),
                HttpStatusCode.NotFound when !string.IsNullOrEmpty(parsedMessage) =>
                    new Exception(parsedMessage),
                _ => new Exception(
                    $"Failed to execute request. Status: {statusCode} Response: {content}")
            };
        }
    }
}
