using System;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.BL.Polygon
{
    public interface IWebSocket : IDisposable
    {
        bool IsDisconnected { get; }

        Task OpenAsync(Uri url, CancellationToken cancellationToken);

        Task CloseAsync(CancellationToken cancellationToken);

        Task SendAsync(string message, CancellationToken cancellationToken);


        public event Action<string> MessageReceived;

        public event Func<Exception, Task> Error;
    }
}
