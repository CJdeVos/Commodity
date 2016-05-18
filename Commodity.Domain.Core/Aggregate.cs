using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Commodity.Domain.Core.Events;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public abstract class Aggregate
    {
        protected Aggregate()
        {
        }

        private AggregateStateEnum _state = AggregateStateEnum.None;
        internal void SetAggregateState(AggregateStateEnum state)
        {
            _state = state;
        }

        private void ApplyTypedEvent(Type t)
        {
            var type = t.MakeGenericType(this.GetType());
            object instance = type.GetConstructor(new Type[0]).Invoke(new object[0]);
            this.ApplyEvent((dynamic)instance);
        }

        protected Aggregate(Guid aggregateId)
        {
            AggregateId = aggregateId;
            //if(_state == AggregateStateEnum.None) // e.g. not replaying
            //    ApplyTypedEvent(typeof(Created<>));
        }

        /*public void Delete()
        {
            //ApplyTypedEvent(typeof (Deleted<>));
        }*/

        public Guid AggregateId { get; internal set; }
        public int CommittedVersion { get; internal set; }
        
        internal protected void ApplyEvent(IAggregateEvent @event)
        {
            var m = this.GetType().GetMethod("Handle", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new[] { @event.GetType() }, null);
            if (m == null) throw new NotImplementedException(String.Format("Handle({0} @event) not implemented for type {1}.", @event.GetType().ToString(), this.GetType().ToString()));
            m.Invoke(this, new[] { @event });
            _uncommittedEvents.Add(@event);
        }

        private readonly List<IAggregateEvent> _uncommittedEvents = new List<IAggregateEvent>();
        internal IEnumerable<IAggregateEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        internal void Commit()
        {
            CommittedVersion += _uncommittedEvents.Count;
            _uncommittedEvents.Clear();
            
        }
    }
}
