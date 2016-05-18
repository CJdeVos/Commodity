using System;
using Commodity.Domain.Core;

namespace Commodity.Serialization
{
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