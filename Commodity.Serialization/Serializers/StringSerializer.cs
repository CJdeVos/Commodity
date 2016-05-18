using System;
using Commodity.Domain.Core;

namespace Commodity.Serialization.Serializers
{
    public class StringSerializer : CommoditySerializer<String>
    {
        public override string Deserialize(ICommodityReader reader)
        {
            return reader.ReadString();
        }

        public override void Serialize(ICommodityWriter writer, string value)
        {
            writer.WriteString(value);
        }
    }
}