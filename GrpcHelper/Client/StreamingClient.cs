using System;
using Grpc.Core;
using GrpcHelper.Common;

namespace GrpcHelper.Client
{
    public class StreamingClient<TReq, TRes> : IDisposable
        where TReq : class
        where TRes : class
    {
        private readonly Channel _channel;
        private readonly RetrySubscriber _mgr;
        private readonly Method<TReq, TRes> _method;

        public StreamingClient(string host, int port, string service, string name, Action<bool> eventHandler = null)
            : this(host, port, MethodFactory.CreateMethod<TReq, TRes>(service, name, MethodType.ServerStreaming), eventHandler) { }

        public StreamingClient(string host, int port, Method<TReq, TRes> method, Action<bool> eventHandler = null)
        {
            _channel = new Channel(host, port, ChannelCredentials.Insecure);
            _mgr = new RetrySubscriber(_channel, eventHandler);
            _method = method;
        }

        public void Subscribe(object id, TReq req, Action<TRes> onResponse, Action<Exception> onError)
        {
            _mgr.Subscribe(id, cancel => new DefaultCallInvoker(_channel).Call(_method, req, cancel, onResponse, onError));
        }

        public void Unsubscribe(object id)
        {
            _mgr.Unsubscribe(id);
        }

        public void Dispose()
        {
            _mgr.UnsubscribeAll();
            _channel.ShutdownAsync().Wait();
        }
    }

    public class StreamingClient : StreamingClient<Tuple<string, string[]>, string>
    {
        public StreamingClient(string host, int port, string service, string name, Action<bool> eventHandler = null)
            : base(host, port, service, name, eventHandler) { }

        public void CallCredentials(object id, string key, string[] args, Action<string> onResponse)
        {
            Subscribe(id, Tuple.Create(key, args), onResponse, e => onResponse(e.Message));
        }
    }
}
