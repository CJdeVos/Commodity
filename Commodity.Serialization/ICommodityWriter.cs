using System;

namespace Commodity.Domain.Core
{
    public interface ICommodityWriter
    {
        ICommodityWriter WriteStartOfObject();
        ICommodityWriter WriteEndOfObject();
        ICommodityWriter WriteName(string name);
        ICommodityWriter WriteType(Type type);
        ICommodityWriter WriteNull();
        ICommodityWriter WriteString(string value);
    }
}