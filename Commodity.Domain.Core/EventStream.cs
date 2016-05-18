using System;
using System.Collections;
using System.Collections.Generic;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class EventStream : IEnumerable<IAggregateEvent>
    {
        private readonly IEnumerator<IAggregateEvent> _enumerator;
        internal EventStream(IEnumerator<IAggregateEvent> enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<IAggregateEvent> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        public static EventStream FromEnumerator(IEnumerator<IAggregateEvent> enumerator)
        {
            return new EventStream(enumerator);
        }
    }
}
