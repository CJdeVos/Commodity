using System;
using System.Collections.Generic;

namespace Commodity.Interfaces
{
    public interface IEventStorage
    {
        IEnumerable<IAggregateEvent> GetEventStream(IAggregateRootId aggregateRootId);

        void Persist(IAggregateRootId aggregateRootId, IEnumerable<IAggregateEvent> events);
    }
}
