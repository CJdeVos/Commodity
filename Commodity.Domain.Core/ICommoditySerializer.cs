using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Commodity.Domain.Core
{
    public static class CommodityBsonSerializer
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

        public static void RegisterSerializer(Type serializerForType, ICommoditySerializer serializer, uint priority = 1)
        {
            Func<Type, bool> fnApplies = (t) => t == serializerForType;
            RegisterSerializer(fnApplies, serializer, priority);
        }

        public static void RegisterSerializer(Func<Type, bool> fnApplies, ICommoditySerializer serializer, uint priority = 1)
        {
            Serializers.Add(priority, new SerializerInfo()
            {
                Applies = fnApplies,
                Serializer = serializer
            });
        }

        private static readonly Dictionary<RuntimeTypeHandle, ICommoditySerializer> ResolvedTypeSerializers = new Dictionary<RuntimeTypeHandle, ICommoditySerializer>();
        public static ICommoditySerializer Resolve(Type t)
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
                throw new NotImplementedException(String.Format("No CommodityBsonSerializer found for object of type {0}", nominalType.FullName));
            }
            return o;
        }

        public static void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            ICommoditySerializer o = EnsureSerializer(nominalType);
            o.Serialize(writer, nominalType, value);
        }

        public static void Serialize<T>(ICommodityWriter writer, T value)
        {
            Serialize(writer, typeof(T), value);
        }

        public static T Deserialize<T>(ICommodityReader reader)
        {
            var nominalType = typeof (T);
            ICommoditySerializer o = EnsureSerializer(nominalType);
            return (T)o.Deserialize(reader, nominalType);
        }
    }

    public interface ICommoditySerializer 
    {
        void Serialize(ICommodityWriter writer, Type nominalType, object value);
        object Deserialize(ICommodityReader reader, Type nominalType);
    }

    public abstract class CommoditySerializer<T> : ICommoditySerializer
    {
        public abstract void Serialize(ICommodityWriter writer, T o);
        public abstract T Deserialize(ICommodityReader reader);

        void ICommoditySerializer.Serialize(ICommodityWriter writer, Type nominalType, object o)
        {
            this.Serialize(writer, (T)o);
        }

        object ICommoditySerializer.Deserialize(ICommodityReader reader, Type nominalType)
        {
            return Deserialize(reader);
        }
    }
}