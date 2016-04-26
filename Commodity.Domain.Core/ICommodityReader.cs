using System;
using MongoDB.Bson;

namespace Commodity.Domain.Core
{
    public interface ICommodityReader
    {
        BsonType GetCurrentBsonType();
        void ReadStartOfObject();
        void ReadEndOfObject();
        string ReadName();
        Type ReadType();
        void ReadNull();
        string ReadString();
    }
}