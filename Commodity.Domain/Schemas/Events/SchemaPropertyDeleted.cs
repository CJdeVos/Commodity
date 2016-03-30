using Commodity.Interfaces;

namespace Commodity.Domain.Schemas.Events
{
    public class SchemaPropertyDeleted : IAggregateEvent
    {
        public string PropertyName { get; set; }
    }
}