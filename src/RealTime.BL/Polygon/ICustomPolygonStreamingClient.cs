using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.BL.Polygon
{
    public interface ICustomPolygonStreamingClient : IDisposable
    {
        bool IsDisconnected { get; }

        event Action<AggregatedPrice> MinuteAggReceived;

        event Func<Exception, Task> OnError;

        event Action<AggregatedPrice> SecondAggReceived;

        Task Connect(CancellationToken cancellationToken = default);

        Task DisconnectAsync(CancellationToken cancellationToken = default);

        void SubscribeMinuteAgg(IEnumerable<string> symbols);

        void UnSubscribeMinuteAgg(IEnumerable<string> symbols);

        void SubscribeSecondAgg(IEnumerable<string> symbols);

        void UnSubscribeSecondAgg(IEnumerable<string> symbols);
    }
}