using System;
using MongoDB.Bson.IO;

namespace Commodity.Domain.Core
{
    public class BsonCommodityWriter : ICommodityWriter
    {
        private readonly IBsonWriter _writer;
        public BsonCommodityWriter(IBsonWriter writer)
        {
            _writer = writer;
        }

        public ICommodityWriter WriteStartOfObject()
        {
            _writer.WriteStartDocument();
            return this;
        }

        public ICommodityWriter WriteEndOfObject()
        {
            _writer.WriteEndDocument();
            return this;
        }

        public ICommodityWriter WriteName(string name)
        {
            _writer.WriteName(name);
            return this;
        }

        public ICommodityWriter WriteType(Type type)
        {
            _writer.WriteString(type.FullName);
            return this;
        }

        public ICommodityWriter WriteNull()
        {
            _writer.WriteNull();
            return this;
        }

        public ICommodityWriter WriteString(string value)
        {
            _writer.WriteString(value);
            return this;
        }

        public ICommodityWriter WriteResolved(Type nominalType, object value)
        {
            CommoditySerializer.Serialize(this, nominalType, value);
            return this;
        }
    }
}