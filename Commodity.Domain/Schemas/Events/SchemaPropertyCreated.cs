using Commodity.Domain.Core;
using Commodity.Interfaces;
using Commodity.Serialization;

namespace Commodity.Domain.Schemas.Events
{
    [CommoditySerializable("SchemaPropertyCreated")]
    public class SchemaPropertyCreated : IAggregateEvent
    {
        public string PropertyName { get; set; }
    }
}