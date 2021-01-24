using Grpc.Core;
using GrpcHelper.Common;

namespace GrpcHelper.Server
{
    using Builder = ServerServiceDefinition.Builder;

    static class BuilderExtension
    {
        public static Builder AddTextUnary(this Builder builder, string service, string name, UnaryServerMethod<string, string> function)
        {
            return builder.AddMethod(MethodFactory.CreateTextMethod(service, name), function);
        }

        public static Builder AddUnary<TReq, TRes>(this Builder builder, string service, string name, UnaryServerMethod<TReq, TRes> function)
            where TReq : class
            where TRes : class
        {
            return builder.AddMethod(MethodFactory.CreateMethod<TReq, TRes>(service, name), function);
        }

        public static Builder AddStreaming<TReq, TRes>(this Builder builder, string service, string name, ServerStreamingServerMethod<TReq, TRes> function)
            where TReq : class
            where TRes : class
        {
            return builder.AddMethod(MethodFactory.CreateMethod<TReq, TRes>(service, name, MethodType.ServerStreaming), function);
        }
    }
}
