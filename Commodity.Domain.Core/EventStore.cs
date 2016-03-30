using System;
using Commodity.Interfaces;
using System.Linq;

namespace Commodity.Domain.Core
{
    public class EventStore : IEventStore
    {
        private readonly IEventStorage _eventStorage;
        private readonly IEventPublisher _eventPublisher;
        public EventStore(IEventStorage eventStorage, IEventPublisher eventPublisher)
        {
            _eventStorage = eventStorage;
            _eventPublisher = eventPublisher;
        }

        private TAggregateRoot CreateNewAggregateRoot<TAggregateRoot>(AggregateRootId<TAggregateRoot> id)
            where TAggregateRoot : IAggregateRoot
        {
            var constructorInfo = typeof(TAggregateRoot).GetConstructor(new[] { typeof(AggregateRootId<TAggregateRoot>) });
            if (constructorInfo == null)
                throw new Exception("Constructor of aggregate root to pass id not found.");

            TAggregateRoot aggregateRoot = (TAggregateRoot)constructorInfo.Invoke(new[] { id });
            return aggregateRoot;
        }

        public TAggregateRoot Load<TAggregateRoot>(IAggregateRootId aggregateRootId) 
            where TAggregateRoot : IAggregateRoot
        {
            if (aggregateRootId == null)
                throw new ArgumentNullException("aggregateRootId");
            var castedAggregateRootId = aggregateRootId as AggregateRootId<TAggregateRoot>;
            if(castedAggregateRootId == null)
                throw new ArgumentException("Must have been an AggregateRootId<TAggregateRoot> object");

            var eventStream = _eventStorage.GetEventStream(aggregateRootId);
            if (eventStream == null)
                return default(TAggregateRoot);

            return CreateNewAggregateRoot(castedAggregateRootId);

            // without playing?
            // play events?
            //aggr.Replay(eventStream);

            //return aggregateRoot;
        }

        public TAggregateRoot LoadNew<TAggregateRoot>() where TAggregateRoot : IAggregateRoot
        {
            return CreateNewAggregateRoot(new AggregateRootId<TAggregateRoot>(Guid.NewGuid()));
        }

        public void Persist<TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TAggregateRoot : IAggregateRoot
        {
            if (aggregateRoot == null)
                throw new ArgumentNullException("aggregateRoot");

            AggregateRoot aggr = aggregateRoot as AggregateRoot;
            if(aggr == null)
                throw new Exception("Aggregate must derive from AggregateRoot");

            // test for uncommitted save unsaved changes
            var uncommittedEvents = aggr.GetUncommittedEvents().ToArray();
            if (uncommittedEvents.Any() == false)
                return;

            _eventStorage.Persist(aggregateRoot.Id, uncommittedEvents);

            // publish events
            _eventPublisher.Publish(aggregateRoot.Id, uncommittedEvents);

            // commit actual aggregate
            aggr.Commit();
            //throw new NotImplementedException();
        }
    }
}
