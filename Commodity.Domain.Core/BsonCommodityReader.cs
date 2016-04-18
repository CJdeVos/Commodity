using MongoDB.Bson.IO;

namespace Commodity.Domain.Core
{
    public class BsonCommodityReader : ICommodityReader
    {
        private readonly IBsonReader _reader;
        public BsonCommodityReader(IBsonReader reader)
        {
            _reader = reader;
        }
    }
}