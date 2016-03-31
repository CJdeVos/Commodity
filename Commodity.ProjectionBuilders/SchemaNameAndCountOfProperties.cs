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

        private static Dictionary<Guid, int> results = new Dictionary<Guid, int>();


        public void Handle(EventContext<Created<Schema>> context)
        {
            EnsureSchemaId(context.AggregateId);
        }

        public void Handle(EventContext<SchemaPropertyCreated> context)
        {
            results[context.AggregateId] += 1;
        }

        public void Handle(EventContext<SchemaPropertyDeleted> context)
        {
            results[context.AggregateId] -= 1;
        }


        private void EnsureSchemaId(Guid aggregateId)
        {
            if (!results.ContainsKey(aggregateId))
                results.Add(aggregateId, 0);
        }

    }
}
