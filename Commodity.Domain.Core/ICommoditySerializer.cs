using System;

namespace Commodity.Domain.Core
{
    public interface ICommoditySerializer
    {
        void Serialize(ICommodityWriter writer, object o);
        object Deserialize(ICommodityReader reader);
    }

    public interface ICommoditySerializer<T>
    {
        void Serialize(ICommodityWriter writer, T o);
        T Deserialize(ICommodityReader reader);
    }
}