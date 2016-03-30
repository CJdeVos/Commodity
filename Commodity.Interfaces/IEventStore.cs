namespace Commodity.Interfaces
{
    public interface IEventStore
    {
        TAggregateRoot Load<TAggregateRoot>(IAggregateRootId aggregateRootId) where TAggregateRoot: IAggregateRoot;
        TAggregateRoot LoadNew<TAggregateRoot>() where TAggregateRoot : IAggregateRoot;
        void Persist<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : IAggregateRoot;
    }
}
