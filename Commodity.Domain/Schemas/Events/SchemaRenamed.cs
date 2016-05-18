using Commodity.Interfaces;
using Commodity.Serialization;

namespace Commodity.Domain.Schemas.Events
{
    [CommoditySerializable("SchemaRenamed")]
    public class SchemaRenamed : IAggregateEvent
    {
        public string SchemaName { get; set; }
    }
}