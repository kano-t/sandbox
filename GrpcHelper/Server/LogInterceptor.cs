using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcHelper.Server
{
    class LogInterceptor : Interceptor
    {
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncServerStreamingCall<TRequest, TResponse>(request, context, continuation);
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            Console.WriteLine(string.Join(" ", DateTime.Now, context.Method, context.Peer, context.AuthContext.PeerIdentityPropertyName));
            return base.UnaryServerHandler<TRequest, TResponse>(request, context, continuation);
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, Interceptor.AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.AsyncUnaryCall<TRequest, TResponse>(request, context, continuation);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            return base.BlockingUnaryCall<TRequest, TResponse>(request, context, continuation);
        }
    }
}
