using Commodity.Interfaces;

namespace Commodity.Domain.Core.Interfaces
{
    public interface IEventHandler<TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Handle(EventContext<TAggregateEvent> context);
    }
}
