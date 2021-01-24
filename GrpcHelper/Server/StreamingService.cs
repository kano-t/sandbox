using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcHelper.Server
{
    public class StreamingService<TReq, TRes>
        where TReq : class
        where TRes : class
    {
        private readonly CompositeDictionary<string, int, IServerStreamWriter<TRes>> _writers = new CompositeDictionary<string, int, IServerStreamWriter<TRes>>();
        private readonly Func<TReq, string> _getKey;

        public event Action<string, TReq> OnSubscribe = (key, value) => { };
        public event Action<string> OnUnsubscribe = key => { };

        public IEnumerable<string> Keys { get { return _writers.Keys; } }

        public StreamingService(Func<TReq, string> getKey)
        {
            _getKey = getKey;
        }

        public async Task OnRequest(TReq request, IServerStreamWriter<TRes> responseStream, ServerCallContext context)
        {
            var id = request.GetHashCode();
            var key = _getKey(request);
            if (_writers.TryAdd(key, id, responseStream))
            {
                OnSubscribe(key, request);
                await CreateCancelTask(context.CancellationToken).ContinueWith(t => Unsubscribe(key, id), TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void Unsubscribe(string key, int id)
        {
            if (_writers.TryRemove(key, id))
            {
                OnUnsubscribe(key);
            }
        }

        private static async Task CreateCancelTask(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            using (token.Register(() => completion.SetResult(null)))
            {
                await completion.Task;
            }
        }

        public void Publish(string key, TRes res)
        {
            Parallel.ForEach(_writers.GetValues(key), writer => WriteSync(writer, res));
        }

        private static void WriteSync<T>(IServerStreamWriter<T> writer, T msg)
        {
            lock (writer)
            {
                writer.WriteAsync(msg).Wait();
            }
        }
    }

    public class StreamingService : StreamingService<Tuple<string, string[]>, string>
    {
        public StreamingService() : base(tuple => tuple.Item1) { }
    }
}
