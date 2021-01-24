using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Grpc.Core;

namespace GrpcHelper.Common
{
    static class MethodFactory
    {
        public static Method<string, string> CreateTextMethod(string service, string name, Encoding encoding = null, MethodType type = MethodType.Unary)
        {
            encoding = encoding ?? Encoding.UTF8;
            var marshaller = new Marshaller<string>(encoding.GetBytes, encoding.GetString);
            return new Method<string, string>(type, service, name, marshaller, marshaller);
        }
        
        public static Method<byte[], byte[]> CreateBinaryMethod(string service, string name, MethodType type = MethodType.Unary)
        {
            var marshaller = new Marshaller<byte[]>(b => b, b => b);
            return new Method<byte[], byte[]>(type, service, name, marshaller, marshaller);
        }

        public static Method<TReq, TRes> CreateMethod<TReq, TRes>(string service, string name, MethodType type = MethodType.Unary)
        {
            var marshallerIn = new Marshaller<TReq>(BinaryConverter.Serialize, BinaryConverter.Deserialize<TReq>);
            var marshallerOut = new Marshaller<TRes>(BinaryConverter.Serialize, BinaryConverter.Deserialize<TRes>);
            return new Method<TReq, TRes>(type, service, name, marshallerIn, marshallerOut);
        }
        /*
        public static Method<TReq, TRes> CreateProtobufMethod<TReq, TRes>(string service, string name, MethodType type = MethodType.Unary)
            where TReq : IMessage, new()
            where TRes : IMessage, new()
        {
            var marshallerIn = new Marshaller<TReq>(obj => obj.ToByteArray(), b => new TReq().Merge(b));
            var marshallerOut = new Marshaller<TRes>(obj => obj.ToByteArray(), b => new TRes().Merge(b));
            return new Method<TReq, TRes>(type, service, name, marshallerIn, marshallerOut);
        }

        private static T Merge<T>(this T obj, byte[] bytes) where T : IMessage
        {
            obj.MergeFrom(bytes);
            return obj;
        }
        */ 
    }

    static class BinaryConverter
    {
        public static byte[] Serialize<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return (T)new BinaryFormatter().Deserialize(stream);
            }
        }
    }
}
