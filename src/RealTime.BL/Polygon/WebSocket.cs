using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.BL.Polygon
{
    public class WebSocket : IWebSocket
    {
        private ClientWebSocket _webSocket;
        private readonly ILogger _logger;

        private Task _recieveTask;
        private CancellationTokenSource _recieveCancellationTokenSource;

        public WebSocket(ILogger<WebSocket> logger)
        {
            _logger = logger;
        }

        public bool IsDisconnected => _webSocket.State != WebSocketState.Open;

        public void Dispose()
        {
            StopReciving();
            _webSocket?.Dispose();
            _webSocket = null;
        }

        public async Task OpenAsync(Uri url, CancellationToken cancellationToken)
        {
            if (_webSocket != null)
            {
                if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting)
                {
                    return;
                }

                _webSocket.Dispose();
                _webSocket = null;
            }

            StopReciving();

            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(url, cancellationToken);
            if (_webSocket.State != WebSocketState.Open)
            {
                throw new Exception($"Can't connect. State: {_webSocket.State}");
            }

            StartReciving();
        }

        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            StopReciving();
            await _webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
        }

        public Task SendAsync(string message, CancellationToken cancellationToken)
        {
            if (_webSocket == null)
            {
                throw new InvalidOperationException("Need to connect first");
            }

            _logger.LogDebug("Message sending: " + message);
            return _webSocket.SendAsync(
                System.Text.Encoding.UTF8.GetBytes(message),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }


        public event Action<string> MessageReceived;

        public event Func<Exception, Task> Error;

        private void StartReciving()
        {
            _recieveCancellationTokenSource = new CancellationTokenSource();
            var token = _recieveCancellationTokenSource.Token;
            _recieveTask = Task.Run(
                async () =>
                {
                    try
                    {
                        while (!token.IsCancellationRequested)
                        {
                            var messageBytes = await ReceiveMessageAsync();
                            var message = System.Text.Encoding.UTF8.GetString(messageBytes);
                            _logger.LogDebug($"Message recieved: {message}");
                            MessageReceived?.Invoke(message);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        await Error?.Invoke(e);
                    }
                },
                token);
        }

        public async Task<byte[]> ReceiveMessageAsync()
        {
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult recieveResult;
                do
                {
                    var receiveBuffer = new ArraySegment<byte>(new byte[1024 * 1024]);
                    recieveResult = await _webSocket.ReceiveAsync(receiveBuffer, CancellationToken.None).ConfigureAwait(false);
                    if (recieveResult.MessageType == WebSocketMessageType.Close)
                    {
                        return new byte[0];
                    }

                    if (recieveResult.MessageType != WebSocketMessageType.Text)
                    {
                        throw new NotImplementedException($"Message type {recieveResult.MessageType} not supported");
                    }

                    if (recieveResult.CloseStatus != null)
                    {
                        throw new Exception(
                            "Socket closed. "
                            + $"Status: {recieveResult.CloseStatus}, Description: {recieveResult.CloseStatusDescription}");
                    }

                    ms.Write(receiveBuffer.Array, receiveBuffer.Offset, recieveResult.Count);
                }
                while (!recieveResult.EndOfMessage);

                return ms.ToArray();
            }
        }

        private void StopReciving()
        {
            _recieveCancellationTokenSource?.Cancel();
            _recieveTask?.Wait(TimeSpan.FromSeconds(10));
            _recieveCancellationTokenSource?.Dispose();
            _recieveCancellationTokenSource = null;
        }
    }
}
