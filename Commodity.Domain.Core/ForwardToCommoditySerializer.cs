using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Commodity.Domain.Core
{
    public class ForwardToCommoditySerializer<T> : SerializerBase<T>
    {
        //private readonly ICommoditySerializer<T> _commoditySerializer;
        public ForwardToCommoditySerializer() //ICommoditySerializer<T> commoditySerializer)
        {
            //_commoditySerializer = commoditySerializer;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            ICommodityWriter commodityWriter = new BsonCommodityWriter(context.Writer);
            CommoditySerializer.Serialize(commodityWriter, typeof(T), value);
        }

        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            ICommodityReader commodityReader = new BsonCommodityReader(context.Reader);
            return CommoditySerializer.Deserialize<T>(commodityReader);
        }
    }
}