using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcHelper.Common;

namespace GrpcHelper.Client
{
    public class UnaryClient
    {
        private readonly Channel _channel;

        public UnaryClient(string host, int port)
        {
            _channel = new Channel(host, port, ChannelCredentials.Insecure);
        }

        public Task<string> Call(string service, string name, string req)
        {
            return Call(MethodFactory.CreateTextMethod(service, name), req);
        }

        public Task<TRes> Call<TReq, TRes>(string service, string name, TReq req)
            where TReq : class
            where TRes : class
        {
            return Call(MethodFactory.CreateMethod<TReq, TRes>(service, name), req);
        }

        protected Task<TRes> Call<TReq, TRes>(Method<TReq, TRes> method, TReq req, CancellationToken cancel = default(CancellationToken))
            where TReq : class
            where TRes : class
        {
            return new DefaultCallInvoker(_channel).AsyncUnaryCall(method, null, new CallOptions(cancellationToken: cancel), req).ResponseAsync;
        }

        public void Dispose()
        {
            _channel.ShutdownAsync().Wait();
        }
    }
}
