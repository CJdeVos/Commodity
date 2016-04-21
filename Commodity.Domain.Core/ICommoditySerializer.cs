using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Commodity.Domain.Core
{
    public static class CommoditySerializer
    {
        private class DescendingSerializerComparer<T> : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                return Comparer<T>.Default.Compare(y, x);
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

        public static void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            ICommoditySerializer o = Resolve(nominalType);
            if (o == null){
                throw new NotImplementedException(String.Format("No CommoditySerializer found for object of type {0}", nominalType.FullName));
            }
            o.Serialize(writer, value);
        }

        public static T Deserialize<T>(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICommoditySerializer 
    {
        void Serialize(ICommodityWriter writer, object o);
        object Deserialize(ICommodityReader reader);
    }

    public abstract class CommoditySerializer<T> : ICommoditySerializer
    {
        public abstract void Serialize(ICommodityWriter writer, T o);
        public abstract T Deserialize(ICommodityReader reader);

        void ICommoditySerializer.Serialize(ICommodityWriter writer, object o)
        {
            this.Serialize(writer, (T) o);
        }

        object ICommoditySerializer.Deserialize(ICommodityReader reader)
        {
            return Deserialize(reader);
        }
    }

    //public abstract class CommoditySerializer : ICommoditySerializer
    //{
    //    public abstract void Serialize(ICommodityWriter writer, object o);
    //    public abstract T Deserialize<T>(ICommodityReader reader);
    //}

    //public abstract class CommoditySerializer<T> : CommoditySerializer, ICommoditySerializer<T>
    //{
    //    public override void Serialize(ICommodityWriter writer, object o){
    //        Serialize(writer, (T)o);
    //    }

    //    public abstract void Serialize(ICommodityWriter writer, T o);
    //    public abstract T Deserialize(ICommodityReader reader);
    //}

}