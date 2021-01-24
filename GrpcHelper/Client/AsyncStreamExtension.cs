using Grpc.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcHelper.Client
{
    static class AsyncStreamExtension
    {
        public static async Task ReadStream<T>(this AsyncServerStreamingCall<T> stream, CancellationToken cancel, Action<T> callback)
        {
            using (stream)
            {
                await stream.ResponseStream.ReadStream(cancel, callback);
            }
        }

        public static async Task ReadStream<T>(this IAsyncStreamReader<T> stream, CancellationToken cancel, Action<T> callback)
        {
            try
            {
                while (await stream.MoveNext(cancel))
                {
                    callback(stream.Current);
                }
            }
            catch (RpcException e)
            {
                switch (e.StatusCode)
                {
                    case StatusCode.Cancelled:
                        return;
                    default:
                        throw;
                }
            }
        }
    }

    static class InvokerExtension
    {
        public static Task Call<TReq, TRes>(this CallInvoker invoker, Method<TReq, TRes> method, TReq req, CancellationToken cancel, Action<TRes> callback, Action<Exception> onError)
            where TReq : class
            where TRes : class
        {
            return invoker.AsyncServerStreamingCall(method, null, new CallOptions(cancellationToken: cancel), req)
                .ReadStream(cancel, res => callback(res))
                .ContinueWith(t => onError(t.Exception.GetBaseException()), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
