using System;
using System.Collections.Generic;
using System.Linq;
using Commodity.Domain.Core;
using Commodity.Domain.Core.Interfaces;
using Commodity.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Shouldly;

namespace Commodity.CommandHandlers.Test.Schemas
{
    [TestClass]
    public class AggregateRepositoryTest
    {
        protected class RecordedEvent
        {
            public Guid AggregateId { get; set; }
            public IAggregateEvent Event { get; set; }
        }

        private IEventPublisher _eventPublisher;
        protected IAggregateRepository AggregateRepository { get; private set; }
        protected TestEventStore EventStore { get; private set; }
        private IEventStreamNameResolver _eventStreamNameResolver;
        protected List<RecordedEvent> RecordedEvents = new List<RecordedEvent>();

        [TestInitialize]
        public void BaseInitialize()
        {
            // Create
            _eventStreamNameResolver = new EventStreamNameResolver();
            EventStore = new TestEventStore();
            _eventPublisher = MockRepository.GenerateMock<IEventPublisher>();
            AggregateRepository = new AggregateRepository(EventStore, _eventStreamNameResolver, _eventPublisher);

            // Set up
            _eventPublisher.Expect(p => p.Publish(Arg<Guid>.Is.Anything, Arg<IEnumerable<IAggregateEvent>>.Is.Anything))
                .WhenCalled(
                    (invocation) =>
                    {
                        Guid aggregateId = (Guid) invocation.Arguments[0];
                        IEnumerable<IAggregateEvent> @events = invocation.Arguments[1] as IEnumerable<IAggregateEvent>;
                        RecordedEvents.AddRange(
                            (@events ?? new IAggregateEvent[0]).Select(
                                @event => new RecordedEvent() {AggregateId = aggregateId, Event = @event}));
                    });
        }

        protected void AssertEventsRecorded(params Action<IAggregateEvent>[] actions)
        {
            // check nr of events
            RecordedEvents.Count.ShouldBe(actions.Count());

            // check order of aggregate events
            int i = 0;
            foreach (var recordedEvent in RecordedEvents)
            {
                actions[i++](recordedEvent.Event);
            }
        }
    }
}