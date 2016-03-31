using System;
using System.Collections.Generic;

namespace Commodity.Interfaces
{
    public interface IEventStorage
    {
        IEnumerable<IAggregateEvent> GetEventStream(Guid aggregateRootId);

        void Persist(Guid aggregateRootId, IEnumerable<IAggregateEvent> events);
    }
}
