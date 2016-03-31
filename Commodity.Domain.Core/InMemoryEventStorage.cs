using System;
using System.Collections.Generic;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class InMemoryEventStorage : IEventStorage
    {

        public IEnumerable<IAggregateEvent> GetEventStream(Guid aggregateRootId)
        {
            return _storage.ContainsKey(aggregateRootId)
                ? _storage[aggregateRootId].AsReadOnly()
                : null;
        }

        private readonly Dictionary<Guid, List<IAggregateEvent>> _storage = new Dictionary<Guid, List<IAggregateEvent>>();
        public void Persist(Guid aggregateRootId, IEnumerable<IAggregateEvent> events)
        {
            if(aggregateRootId==null)
                throw new ArgumentNullException("aggregateRootId");

            if(!_storage.ContainsKey(aggregateRootId))
                _storage.Add(aggregateRootId, new List<IAggregateEvent>());
            _storage[aggregateRootId].AddRange(events);
        }
    }
}
