using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public abstract class EventContext<TEvent>
        where TEvent: IAggregateEvent
    {
        private EventContext(IAggregateRootId aggregateRootId, TEvent @event)
        {
            this.AggregateRootId = aggregateRootId;
            this.Event = @event;
        }

        public IAggregateRootId AggregateRootId { get; private set; }
        public TEvent @Event {get; private set; }

        
    }
}
