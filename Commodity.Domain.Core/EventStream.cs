using System;
using System.Collections;
using System.Collections.Generic;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class EventData
    {
        
    }

    public class EventStream : IEnumerable<EventData>
    {
        private readonly IEnumerator<EventData> _enumerator;
        internal EventStream(IEnumerator<EventData> enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<EventData> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }
}
