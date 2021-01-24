using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcHelper.Client
{
    class RetrySubscriber
    {
        private readonly ConcurrentDictionary<object, Subscriber> _subscribers = new ConcurrentDictionary<object, Subscriber>();
        private readonly Channel _channel;
        private readonly Task _task;
        
        private struct Subscriber
        {
            public Action<CancellationToken> Action { get; set; }
            public CancellationTokenSource Cancel { get; set; }
        }

        public RetrySubscriber(Channel channel, Action<bool> onStatusChanged = null)
        {
            _channel = channel;
            _task = Run(onStatusChanged ?? (b => { }));
        }

        private async Task Run(Action<bool> onStatusChanged)
        {
            try
            {
                while (_channel.State != ChannelState.Shutdown)
                {
                    await _channel.ConnectAsync().ConfigureAwait(false);
                    onStatusChanged(true);
                    Parallel.ForEach(_subscribers.Values, sub => sub.Action(sub.Cancel.Token));
                    await _channel.WaitForStateChangedAsync(_channel.State).ConfigureAwait(false);
                    onStatusChanged(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Subscribe(object id, Action<CancellationToken> action)
        {
            Unsubscribe(id);
            var sub = _subscribers[id] = new Subscriber { Action = action, Cancel = new CancellationTokenSource() };
            if (_channel.State == ChannelState.Ready)
            {
                sub.Action(sub.Cancel.Token);
            }
        }

        public void Unsubscribe(object id)
        {
            Subscriber sub;
            if (_subscribers.TryRemove(id, out sub))
            {
                sub.Cancel.Cancel();
            }
        }

        public void UnsubscribeAll()
        {
            Parallel.ForEach(_subscribers.Values, sub => sub.Cancel.Cancel());
            _subscribers.Clear();
        }
    }

}
