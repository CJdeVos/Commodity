using System.Collections.Generic;

namespace Commodity.Interfaces
{
    public interface IEventPublisher
    {
        void Publish(IAggregateRootId aggregateRootId, IEnumerable<IAggregateEvent> events);
    }
}
