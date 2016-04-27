using Commodity.Domain.Core;
using Commodity.Interfaces;

namespace Commodity.Domain.Schemas.Events
{
    [CommodityBsonSerializable("SchemaPropertyCreated")]
    public class SchemaPropertyCreated : IAggregateEvent
    {
        public string PropertyName { get; set; }
    }
}