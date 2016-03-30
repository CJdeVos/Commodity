using Commodity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commodity.Domain.Core;
using Commodity.Domain.Core.Events;
using Commodity.Domain.Core.Interfaces;
using Commodity.Domain.Schemas;
using Commodity.Domain.Schemas.Events;

namespace Commodity.ProjectionBuilders
{
    public class SchemaNameAndCountOfProperties : IProjectionBuilder,
            IEventHandler<Created<Schema>>,
            IEventHandler<SchemaPropertyDeleted>,
            IEventHandler<SchemaPropertyCreated>
    {
        public SchemaNameAndCountOfProperties()
        {
        }

        private static Dictionary<IAggregateRootId, int> results = new Dictionary<IAggregateRootId, int>();


        public void Handle(EventContext<Created<Schema>> context)
        {
            EnsureSchemaId(context.AggregateRootId);
        }

        public void Handle(EventContext<SchemaCreated> context)
        {
            EnsureSchemaId(context.AggregateRootId);
        }


        public void Handle(EventContext<SchemaPropertyCreated> context)
        {
            results[context.AggregateRootId] += 1;
        }

        public void Handle(EventContext<SchemaPropertyDeleted> context)
        {
            results[context.AggregateRootId] -= 1;
        }


        private void EnsureSchemaId(IAggregateRootId aggregateId)
        {
            if (!results.ContainsKey(aggregateId))
                results.Add(aggregateId, 0);
        }

    }
}
