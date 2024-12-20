using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealTime.BL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.BL.Polygon
{
    public class CustomPolygonStreamingClient : ICustomPolygonStreamingClient
    {
        private const string MinuteAggChannel = "AM";

        private const string SecondAggChannel = "A";

        private const string StatusMessage = "status";

        private static readonly Uri ApiUrl = new Uri("wss://polyfeed.polygon.io/stocks", UriKind.Absolute);

        private readonly IWebSocket _webSocket;

        private readonly string _apiKey;

        private readonly IDictionary<string, Action<JToken>> _handlers;

        private bool _isAuthenticated;

        /// <summary>
        /// Creates new instance of <see cref="StreamingClientBase{TConfiguration}"/> object.
        /// </summary>
        /// <param name="configuration"></param>
        public CustomPolygonStreamingClient(IWebSocket webSocket, IOptions<AppSettings> settings)
        {
            _webSocket = webSocket;
            _webSocket.MessageReceived += OnMessageReceived;
            _webSocket.Error += HandleError;
            _apiKey = settings.Value.AlpacaPolygonKey;

            _handlers = new Dictionary<string, Action<JToken>>(StringComparer.OrdinalIgnoreCase)
            {
                { StatusMessage, HandleAuthorization },
                { MinuteAggChannel, HandleMinuteAggChannel },
                { SecondAggChannel, HandleSecondAggChannel }
            };
        }

        public bool IsDisconnected => _webSocket.IsDisconnected;

        public event Action<AggregatedPrice> MinuteAggReceived;

        public event Action<AggregatedPrice> SecondAggReceived;

        public event Func<Exception, Task> OnError;

        public async Task Connect(CancellationToken cancellationToken = default)
        {
            _isAuthenticated = false;
            await _webSocket.OpenAsync(ApiUrl, cancellationToken);
            if (!_isAuthenticated)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1500));
                if (!_isAuthenticated)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(1500));
                    if (!_isAuthenticated)
                    {
                        throw new Exception("failed to authneticate - no connect message");
                    }
                }
            }
        }

        /// <summary>
        /// Closes connection to a streaming API.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Awaitable task object for handling action completion in asynchronous mode.</returns>
        public Task DisconnectAsync(CancellationToken cancellationToken = default)
            => _webSocket.CloseAsync(cancellationToken);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SubscribeSecondAgg(IEnumerable<string> symbols) =>
            Subscribe(GetParams(SecondAggChannel, symbols));

        public void UnSubscribeSecondAgg(IEnumerable<string> symbols) =>
            Unsubscribe(GetParams(SecondAggChannel, symbols));

        public void SubscribeMinuteAgg(IEnumerable<string> symbols) =>
            Subscribe(GetParams(MinuteAggChannel, symbols));

        public void UnSubscribeMinuteAgg(IEnumerable<string> symbols) =>
            Unsubscribe(GetParams(MinuteAggChannel, symbols));

        private void OnMessageReceived(string message)
        {
            try
            {
                foreach (var token in JArray.Parse(message))
                {
                    var messageType = token["ev"];
                    if (messageType is null)
                    {
                        HandleError(new InvalidOperationException($"Invalid message: {message}"));
                    }
                    else
                    {
                        HandleMessage(_handlers, messageType.ToString(), token);
                    }
                }
            }
            catch (Exception exception)
            {
                HandleError(exception);
            }
        }

        /// <summary>
        /// Implement <see cref="IDisposable"/> pattern for inheritable classes.
        /// </summary>
        /// <param name="disposing">If <c>true</c> - dispose managed objects.</param>
        protected virtual void Dispose(
            Boolean disposing)
        {
            if (!disposing || _webSocket == null)
            {
                return;
            }

            _webSocket.MessageReceived -= OnMessageReceived;
            _webSocket.Error -= HandleError;
            _webSocket.Dispose();
        }

        private void HandleMessage<TKey>(IDictionary<TKey, Action<JToken>> handlers, TKey messageType, JToken message)
            where TKey : class
        {
            try
            {
                if (handlers != null &&
                    handlers.TryGetValue(messageType, out var handler))
                {
                    handler(message);
                }
                else
                {
                    HandleError(
                        new InvalidOperationException($"Unexpected message type '{messageType}' received."));
                }
            }
            catch (Exception exception)
            {
                HandleError(exception);
            }
        }

        private Task HandleError(Exception exception)
        {
            return OnError?.Invoke(exception);
        }

        private void HandleAuthorization(
            JToken token)
        {
            var connectionStatus = token.ToObject<PolygonConnectionStatus>();

            // ReSharper disable once ConstantConditionalAccessQualifier
            switch (connectionStatus?.Status)
            {
                case ConnectionStatus.Connected:
                    SendAsJsonString
                        (new
                        {
                            action = PolygonAction.PolygonAuthenticate,
                            @params = _apiKey
                        });
                    break;

                case ConnectionStatus.AuthenticationSuccess:
                    _isAuthenticated = true;
                    break;

                case ConnectionStatus.AuthenticationFailed:
                case ConnectionStatus.AuthenticationRequired:
                    HandleError(new InvalidOperationException(connectionStatus.Message));
                    break;

                case ConnectionStatus.Error:
                case ConnectionStatus.Failed:
                case ConnectionStatus.Success:
                    break;

                default:
                    HandleError(new InvalidOperationException("Unknown connection status"));
                    break;
            }
        }

        private void Subscribe(string parameters) =>
            SendAsJsonString(
                new
                {
                    action = PolygonAction.PolygonSubscribe,
                    @params = parameters
                });

        private void Unsubscribe(string parameters) =>
            SendAsJsonString(
                new
                {
                    action = PolygonAction.PolygonUnsubscribe,
                    @params = parameters
                });

        private static string GetParams(string channel, string symbol) =>
            $"{channel}.{symbol}";

        private static string GetParams(string channel, IEnumerable<string> symbols) =>
            string.Join(",", symbols.Select(symbol => GetParams(channel, symbol)));

        private void HandleMinuteAggChannel(JToken token)
        {
            var price = token.ToObject<AggregatedPrice>();
            MinuteAggReceived?.Invoke(price);
        }

        private void HandleSecondAggChannel(
            JToken token)
        {
            var price = token.ToObject<AggregatedPrice>();
            SecondAggReceived?.Invoke(price);
        }

        /// <summary>
        /// Some method.
        /// </summary>
        /// <param name="value">Some value.</param>
        private Task SendAsJsonString(object value)
            => _webSocket.SendAsync(JsonConvert.SerializeObject(value), CancellationToken.None);
    }
}
