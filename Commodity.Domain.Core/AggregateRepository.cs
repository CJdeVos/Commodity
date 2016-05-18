using System;
using System.Linq;
using System.Threading.Tasks;
using Commodity.Domain.Core.Interfaces;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public enum AggregateStateEnum : byte
    {
        None = 0,
        ReplayingEvents = 0
 }

    public class AggregateRepository : IAggregateRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IEventStreamNameResolver _eventStreamNameResolver;
        private readonly IEventPublisher _eventPublisher;

        public AggregateRepository(IEventStore eventStore, IEventStreamNameResolver eventStreamNameResolver, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventStreamNameResolver = eventStreamNameResolver;
            _eventPublisher = eventPublisher;
        }

        public async Task<TAggregate> Load<TAggregate>(Guid aggregateId, int version) where TAggregate : Aggregate
        {
            string streamName = _eventStreamNameResolver.Resolve<TAggregate>(aggregateId);
            

            // load snapshot

            // load stream and apply events
            var eventStream = await _eventStore.GetEventStream(streamName, 0, version);
            if (eventStream!=null)
            {
                TAggregate aggregate = ConstructAggregate<TAggregate>(aggregateId);
                aggregate.SetAggregateState(AggregateStateEnum.ReplayingEvents);
                
                foreach (var @event in eventStream)
                {
                    aggregate.ApplyEvent(@event);
                }
                aggregate.Commit();
                aggregate.SetAggregateState(AggregateStateEnum.None);
                return aggregate;
            }

            // return aggregate
            return null;
        }

        public async Task Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
        {
            if (aggregate == null)
                throw new ArgumentNullException("aggregate");

            // test for uncommitted save unsaved changes
            var uncommittedEvents = aggregate.GetUncommittedEvents().ToArray();
            if (uncommittedEvents.Any() == false)
                return; // Task.FromResult(0);

            string streamName = _eventStreamNameResolver.Resolve<TAggregate>(aggregate.AggregateId);

            //// append events to stream
            //var task = _eventStore.AppendToEventStream(streamName, aggregate.CommittedVersion, uncommittedEvents);
            //return task.ContinueWith((t) => {
            //    _eventPublisher.Publish(aggregate.AggregateId, uncommittedEvents);

            //    // commit aggregate (e.g. reset)
            //    aggregate.Commit();

            //    //return Task.FromResult(0);
            //});
            //// publish events

            
            await _eventStore.AppendToEventStream(streamName, aggregate.CommittedVersion, uncommittedEvents);

            // publish events
            _eventPublisher.Publish(aggregate.AggregateId, uncommittedEvents);

            // commit aggregate (e.g. reset)
            aggregate.Commit();
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
