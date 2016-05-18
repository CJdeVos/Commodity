using System;
using Commodity.Domain.Core;

namespace Commodity.Serialization.Serializers
{
    public class ValueTypeSerializer : ICommoditySerializer
    {
        public void Serialize(ICommodityWriter writer, Type nominalType, object value)
        {
            // must be a struct serializer
            throw new NotImplementedException();
        }

        public object Deserialize(ICommodityReader reader, Type nominalType)
        {
            throw new NotImplementedException();
        }
    }
}