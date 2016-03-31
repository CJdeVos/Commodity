using System;
using System.Collections.Generic;

namespace Commodity.Interfaces
{
    public interface IEventPublisher
    {
        void Publish(Guid aggregateRootId, IEnumerable<IAggregateEvent> events);
    }
}
