using System;
using System.Reflection;
using Commodity.Domain.Core.Events;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class EventContext
    {
        public EventContext(Guid aggregateId, IAggregateEvent @event)
        {
            this.AggregateId = aggregateId;
            this.Event = @event;
        }

        public Guid AggregateId { get; private set; }
        public IAggregateEvent Event { get; private set; }

        internal object ToGenericEventContext()
        {
            Type eventType = Event.GetType();
            var mi = this.GetType().GetMethod("InternalToGenericEventContext", BindingFlags.NonPublic | BindingFlags.Instance);
            var fooRef = mi.MakeGenericMethod(eventType);
            return fooRef.Invoke(this, null);
        }

        private EventContext<TEvent> InternalToGenericEventContext<TEvent>()
            where TEvent : class, IAggregateEvent
        {
            return new EventContext<TEvent>(this);
        }
    }

    public class EventContext<TEvent> : EventContext
        where TEvent : class, IAggregateEvent
    {
        internal EventContext(EventContext context) 
            : base(context.AggregateId, context.Event){
        }

        public new TEvent Event {
            get { return base.Event as TEvent; }
        }

        
    }
}
