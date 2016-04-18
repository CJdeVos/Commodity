using System;

namespace Commodity.Domain.Core
{
    public interface IResolveCommoditySerializer
    {
        ICommoditySerializer<T> Resolve<T>();
        void Serialize(ICommodityWriter writer, Type nominalType, object value);
    }
}