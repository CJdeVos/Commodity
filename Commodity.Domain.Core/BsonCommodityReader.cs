using System;
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

        public void ReadEndOfObject()
        {
            _reader.ReadEndDocument();
        }

        public string ReadName()
        {
            return _reader.ReadName();
        }

        public void ReadNull()
        {
            _reader.ReadNull();
        }


        public void ReadStartOfObject()
        {
            var currentBsonType = _reader.GetCurrentBsonType();
            _reader.ReadStartDocument();
        }

        public string ReadString()
        {
            return _reader.ReadString();
        }

        public Type ReadType()
        {
            //_reader.CurrentBsonType
            throw new NotImplementedException();
        }
    }
}