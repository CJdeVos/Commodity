using System;
using Commodity.Interfaces;
using Shouldly;

namespace Commodity.CommandHandlers.Test.Schemas
{
    public static class AggregateEventAssertExtensions
    {
        public static T Is<T>(this IAggregateEvent aggregateEvent, Func<T, bool> f) where T: class, IAggregateEvent
        {
            T @event = aggregateEvent as T;
            @event.ShouldNotBeNull("Is another type.");
            f(@event).ShouldBeTrue("Is verification is not true.");
            return @event;
        }

        public static T Is<T>(this IAggregateEvent aggregateEvent) where T : class, IAggregateEvent
        {
            return aggregateEvent.Is<T>((t) => true);
        }
    }
}