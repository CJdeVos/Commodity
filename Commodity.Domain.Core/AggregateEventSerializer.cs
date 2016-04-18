using System;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class AggregateEventSerializer : ICommoditySerializer<IAggregateEvent>
    {
        private readonly IResolveCommoditySerializer _resolver;
        public AggregateEventSerializer(IResolveCommoditySerializer resolver)
        {
            _resolver = resolver;
        }

        public void Serialize(ICommodityWriter writer, IAggregateEvent o)
        {
            writer.WriteStartOfObject();
            writer.WriteName("t");
            writer.WriteString(o.GetType().FullName);
            writer.WriteName("t");
            writer.WriteResolved(_resolver, o.GetType(), o);
            writer.WriteEndOfObject();
        }

        public IAggregateEvent Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ICommodityWriter writer, object o)
        {

            throw new NotImplementedException();
        }

        object ICommoditySerializer.Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }
    }
}