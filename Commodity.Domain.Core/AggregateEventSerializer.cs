using System;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class AggregateEventSerializer : CommoditySerializer<IAggregateEvent>
    {
        public override void Serialize(ICommodityWriter writer, IAggregateEvent o)
        {
            writer.WriteStartOfObject();
            writer.WriteName("t");
            writer.WriteString(o.GetType().FullName);
            writer.WriteName("t");
            CommoditySerializer.Serialize(writer, o.GetType(), o);
            writer.WriteEndOfObject();
        }

        public override IAggregateEvent Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultSerializer : ICommoditySerializer
    {
        public void Serialize(ICommodityWriter writer, object o)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }
    }
}