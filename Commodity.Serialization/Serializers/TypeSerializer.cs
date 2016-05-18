using System;
using Commodity.Domain.Core;
using Commodity.Serialization.TypeResolver;

namespace Commodity.Serialization.Serializers
{
    public class TypeSerializer : CommoditySerializer<Type>
    {
        private readonly ICommodityTypeResolver _typeResolver;
        public TypeSerializer(ICommodityTypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public override Type Deserialize(ICommodityReader reader)
        {
            string handle = reader.ReadString();
            return _typeResolver.GetTypeFromHandle(handle);
        }

        public override void Serialize(ICommodityWriter writer, Type o)
        {
            string handle = _typeResolver.GetHandleFromType(o);
            writer.WriteString(handle);
        }
    }
}