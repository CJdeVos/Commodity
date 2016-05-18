using System;

namespace Commodity.Serialization
{
    public class CommoditySerializableAttribute : Attribute
    {
        public CommoditySerializableAttribute(string uniqueId)
        {
            UniqueId = uniqueId;
        }

        public string UniqueId { get; private set; }
    }
}
