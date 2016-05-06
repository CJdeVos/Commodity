using System;
using MongoDB.Bson;
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

        public BsonType ReadBsonType()
        {
            return _reader.ReadBsonType();
        }

        //public BsonType GetCurrentBsonType()
        //{
        //    //ReadBsonType
        //    var currentBsonType = _reader.CurrentBsonType;
        //    var state = _reader.State;
            
        //    //if (currentBsonType == BsonType.EndOfDocument)
        //    //    return BsonType.EndOfDocument;
        //    var newCurrentBsonType = _reader.GetCurrentBsonType();
        //    return newCurrentBsonType;
        //}

        public void ReadStartOfObject()
        {
            _reader.ReadStartDocument();
        }

        public string ReadString()
        {
            return _reader.ReadString();
        }

        public Type ReadType()
        {
            string typeAsString = _reader.ReadString();
            return Type.GetType(typeAsString);
        }
    }
}