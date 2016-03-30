using System;
using System.Collections.Generic;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class InMemoryEventStorage : IEventStorage
    {

        public IEnumerable<IAggregateEvent> GetEventStream(IAggregateRootId aggregateRootId)
        {
            return _storage.ContainsKey(aggregateRootId.TechnicalId)
                ? _storage[aggregateRootId.TechnicalId].AsReadOnly()
                : null;
        }

        private readonly Dictionary<Guid, List<IAggregateEvent>> _storage = new Dictionary<Guid, List<IAggregateEvent>>();
        public void Persist(IAggregateRootId aggregateRootId, IEnumerable<IAggregateEvent> events)
        {
            if(aggregateRootId==null)
                throw new ArgumentNullException("aggregateRootId");

            if(!_storage.ContainsKey(aggregateRootId.TechnicalId))
                _storage.Add(aggregateRootId.TechnicalId, new List<IAggregateEvent>());
            _storage[aggregateRootId.TechnicalId].AddRange(events);
        }
    }
}
