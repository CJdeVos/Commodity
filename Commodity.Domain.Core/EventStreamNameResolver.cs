using System;
using Commodity.Domain.Core.Interfaces;

namespace Commodity.Domain.Core
{
    public class EventStreamNameResolver : IEventStreamNameResolver
    {
        public string Resolve<TAggregate>(Guid aggregateId) where TAggregate : Aggregate
        {
            return aggregateId.ToString();
        }
    }
}
