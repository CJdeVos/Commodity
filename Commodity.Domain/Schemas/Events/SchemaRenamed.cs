using Commodity.Interfaces;

namespace Commodity.Domain.Schemas.Events
{
    public class SchemaRenamed : IAggregateEvent
    {
        public string SchemaName { get; set; }
    }
}