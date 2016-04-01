using System;
using Commodity.Domain.Core.Interfaces;

namespace Commodity.Domain.Core
{
    public class AggregateRepository : IAggregateRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IEventStreamNameResolver _eventStreamNameResolver;
        public AggregateRepository(IEventStore eventStore, IEventStreamNameResolver eventStreamNameResolver)
        {
            _eventStore = eventStore;
            _eventStreamNameResolver = eventStreamNameResolver;
        }

        public TAggregate Load<TAggregate>(Guid aggregateId, int version) where TAggregate : Aggregate
        {
            // load snapshot
            string streamName = _eventStreamNameResolver.Resolve<TAggregate>(aggregateId);
            TAggregate aggregate = ConstructAggregate<TAggregate>(aggregateId);

            var eventStream = _eventStore.GetEventStream(streamName, 0, version);
            foreach (var @event in eventStream)
            {
                aggregate.ApplyEvent(@event);
            }
        }

        public void Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            throw new NotImplementedException();
        }

        private TAggregate ConstructAggregate<TAggregate>(Guid aggregateId)
        {
            var constructorInfo = typeof(TAggregate).GetConstructor(new[] { typeof(Guid) });
            if (constructorInfo == null)
                throw new Exception("Constructor of aggregate root to pass Guid id not found.");

            return (TAggregate)constructorInfo.Invoke(new[] { (object)aggregateId });
        }
    }
}
