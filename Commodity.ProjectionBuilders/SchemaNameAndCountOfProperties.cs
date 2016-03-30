using Commodity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commodity.Domain.Schemas.Events;

namespace Commodity.ProjectionBuilders
{
    public class SchemaNameAndCountOfProperties : IProjectionBuilder,
            IEventHandler<SchemaCreated>,
            IEventHandler<SchemaPropertyDeleted>,
            IEventHandler<SchemaPropertyCreated>
    {
        public SchemaNameAndCountOfProperties()
        {
        }

        private static Dictionary<IAggregateRootId, int> results = new Dictionary<IAggregateRootId, int>();

        public void Handle(IAggregateRootId aggregateId, SchemaCreated @event)
        {
            EnsureSchemaId(aggregateId);
        }


        public void Handle(IAggregateRootId aggregateId, SchemaPropertyCreated @event)
        {
            results[aggregateId] += 1;
        }

        public void Handle(IAggregateRootId aggregateId, SchemaPropertyDeleted @event)
        {
            results[aggregateId] -= 1;
        }


        private void EnsureSchemaId(IAggregateRootId aggregateId)
        {
            if (!results.ContainsKey(aggregateId))
                results.Add(aggregateId, 0);
        }

    }
}
