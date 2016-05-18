using System;
using System.Collections.Generic;
using Commodity.Domain.Core;
using Commodity.Domain.Schemas.Events;
using Commodity.Domain.Core.Events;
using Commodity.Serialization;

namespace Commodity.Domain.Schemas
{
    [CommoditySerializable("Schema")]
    public class Schema : Aggregate
    {
        public Schema() { } // recommended -> but doesn't throw CREATED event
        public Schema(Guid schemaId) : base(schemaId)
        {
        }

        public string Name { get; private set; }
        public readonly List<string> Properties = new List<string>();
        //public IReadOnlyList<string> Properties => _properties.AsReadOnly();

        public void AddProperty(string propertyName)
        {
            ApplyEvent(new SchemaPropertyCreated()
            {
                PropertyName = propertyName
            });
        }

        public void DeleteProperty(string schemaName)
        {
            ApplyEvent(new SchemaPropertyDeleted() { PropertyName = schemaName });
        }

        public void Rename(string name)
        {
            ApplyEvent(new SchemaRenamed(){
                SchemaName = name
            });
        }

        private void Handle(SchemaRenamed @event)
        {
            Name = @event.SchemaName;
        }

        private void Handle(SchemaPropertyCreated @event)
        {
            Properties.Add(@event.PropertyName);
        }

        private void Handle(SchemaPropertyDeleted @event)
        {
            Properties.Remove(@event.PropertyName);
        }

    }
}
