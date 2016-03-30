using Commodity.Interfaces;

namespace Commodity.Domain.Schemas.Events
{
    public class SchemaPropertyCreated : IAggregateEvent
    {
        public string PropertyName { get; set; }
    }
}