using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Commodity.Domain.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Commodity.Serialization
{
    public static class CommoditySerializer
    {
        private class DescendingSerializerComparer<T> : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                int result = Comparer<T>.Default.Compare(y, x);
                if (result == 0)
                    return 1;
                return result;
            }
        }

        private class SerializerInfo
        {
            public Func<Type, bool> Applies { get; set; }
            public ICommoditySerializer Serializer { get; set; }
        }

        private readonly static SortedList<uint, SerializerInfo> Serializers = new SortedList<uint, SerializerInfo>(new DescendingSerializerComparer<uint>());

        internal static void RegisterSerializer(Type serializerForType, ICommoditySerializer serializer, uint priority = 1)
        {
            Func<Type, bool> fnApplies = (t) => t == serializerForType;
            RegisterSerializer(fnApplies, serializer, priority);
        }

        internal static void RegisterSerializer(Func<Type, bool> fnApplies, ICommoditySerializer serializer, uint priority = 1)
        {
            Serializers.Add(priority, new SerializerInfo()
            {
                Applies = fnApplies,
                Serializer = serializer
            });
        }

        private static readonly Dictionary<RuntimeTypeHandle, ICommoditySerializer> ResolvedTypeSerializers = new Dictionary<RuntimeTypeHandle, ICommoditySerializer>();
        internal static ICommoditySerializer Resolve(Type t)
        {
            // check resolved types first
            if (ResolvedTypeSerializers.ContainsKey(t.TypeHandle))
                return ResolvedTypeSerializers[t.TypeHandle];
            var serializer = Serializers.Values.FirstOrDefault(s => s.Applies(t));
            if (serializer == null)
            {
                throw new Exception("No serializer resolved for type.");
            }
            ResolvedTypeSerializers.Add(t.TypeHandle, serializer.Serializer);
            return serializer.Serializer;
        }

        private static ICommoditySerializer EnsureSerializer(Type nominalType)
        {
            ICommoditySerializer o = Resolve(nominalType);
            if (o == null)
            {
                throw new NotImplementedException(String.Format("No CommoditySerializer found for object of type {0}", nominalType.FullName));
            }
            return o;
        }

        internal static void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            ICommoditySerializer o = EnsureSerializer(nominalType);
            o.Serialize(writer, nominalType, value);
        }

        internal static void Serialize<T>(ICommodityWriter writer, T value)
        {
            Serialize(writer, typeof(T), value);
        }

        internal static T Deserialize<T>(ICommodityReader reader)
        {
            var nominalType = typeof (T);
            return (T) Deserialize(reader, typeof (T));
            //ICommoditySerializer o = EnsureSerializer(nominalType);
            //return (T)o.Deserialize(reader, nominalType);
        }

        internal static object Deserialize(ICommodityReader reader, Type nominalType)
        {
            ICommoditySerializer o = EnsureSerializer(nominalType);
            return o.Deserialize(reader, nominalType);
        }

        //public static object Deserialize(BsonDocument document)
        //{
        //    throw new NotImplementedException();
        //}

        public static T Deserialize<T>(BsonDocument document)
        {
            BsonDocumentReader reader = new BsonDocumentReader(document, new BsonDocumentReaderSettings() { GuidRepresentation = GuidRepresentation.Standard });
            ICommodityReader commodityReader = new BsonCommodityReader(reader);
            return CommoditySerializer.Deserialize<T>(commodityReader);
        }

        //public static BsonDocument Serialize(object instance)
        //{
        //    throw new NotImplementedException();
        //}

        public static BsonDocument Serialize<T>(T instance)
        {
            BsonDocument doc = new BsonDocument();
            BsonDocumentWriter writer = new BsonDocumentWriter(doc, new BsonDocumentWriterSettings() { GuidRepresentation = GuidRepresentation.Standard });
            ICommodityWriter commodityWriter = new BsonCommodityWriter(writer);
            CommoditySerializer.Serialize(commodityWriter, instance);
            return doc;
        }
    }
}