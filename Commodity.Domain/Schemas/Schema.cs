using System;
using System.Collections.Generic;
using Commodity.Domain.Core;
using Commodity.Domain.Schemas.Events;
using Commodity.Domain.Core.Events;

namespace Commodity.Domain.Schemas
{
    public class Schema : Aggregate
    {
        public Schema() { }
        public Schema(Guid schemaId) : base(schemaId)
        {
        }

        public string Name { get; private set; }
        private readonly List<string> _properties = new List<string>();
        public IReadOnlyList<string> Properties
        {
            get { return _properties.AsReadOnly(); }
        }

        public void AddProperty(string propertyName)
        {
            Publish(new SchemaPropertyCreated()
            {
                PropertyName = propertyName
            });
        }

        public void DeleteProperty(string schemaName)
        {
            Publish(new SchemaPropertyDeleted() { PropertyName = schemaName });
        }

        private void Handle(Created<Schema> @event)
        {
        }

        private void Handle(SchemaRenamed @event)
        {
            Name = @event.SchemaName;
        }

        private void Handle(SchemaPropertyCreated @event)
        {
            _properties.Add(@event.PropertyName);
        }

        private void Handle(SchemaPropertyDeleted @event)
        {
            _properties.Remove(@event.PropertyName);
        }

    }
}
