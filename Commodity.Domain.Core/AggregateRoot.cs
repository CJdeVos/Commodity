using System;
using System.Collections.Generic;
using System.Reflection;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(IAggregateRootId id)
        {
            Id = id;
        }

        public IAggregateRootId Id { get; internal set; }

        /* play state */
        private bool _playState;
        internal void StartPlayState()
        {
            _playState = true;
        }
        internal void EndPlayState()
        {
            _playState = false;
        }

        internal bool IsPlayState()
        {
            return _playState;
        }
        /* */
        internal protected void Publish(IAggregateEvent @event)
        {
            var m = this.GetType().GetMethod("Handle", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new[] { @event.GetType() }, null);
            if (m == null) throw new NotImplementedException(String.Format("Handle({0} @event) not implemented for type {1}.", @event.GetType().ToString(), this.GetType().ToString()));
            m.Invoke(this, new[] { @event });

            if (!IsPlayState())
                _uncommittedEvents.Add(@event);
        }

        private readonly List<IAggregateEvent> _uncommittedEvents = new List<IAggregateEvent>();
        internal IEnumerable<IAggregateEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents;
        }

        internal void Commit()
        {
            _uncommittedEvents.Clear();
        }
    }
}
