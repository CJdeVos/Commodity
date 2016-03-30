namespace Commodity.Interfaces
{
    public interface IEventHandler<TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Handle(IAggregateRootId id, TAggregateEvent @event);
    }
}
