using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commodity.Domain.Core;
using Commodity.Domain.Core.Interfaces;
using Commodity.Interfaces;

namespace Commodity.CommandHandlers.Test
{
    public class TestEventStore : IEventStore
    {
        public readonly Dictionary<string, List<IAggregateEvent>> Streams = new Dictionary<string, List<IAggregateEvent>>();
        public Task AppendToEventStream(string streamName, int expectedVersion, IEnumerable<IAggregateEvent> events)
        {
            List<IAggregateEvent> stream = null;
            if (!Streams.ContainsKey(streamName))
            {
                stream = new List<IAggregateEvent>();
                Streams.Add(streamName, stream);
                if (expectedVersion > 0)
                    throw new Exception("Stream not found but expected a version>0.");
            }
            else
            {
                stream = Streams[streamName];
                if (stream.Count != expectedVersion)
                    throw new Exception("Invalid version.");
            }
            stream.AddRange(events);
            return Task.FromResult(0x0);
        }

        public Task<EventStream> GetEventStream(string streamName, int startVersion, int? untilVersion)
        {
            if (!Streams.ContainsKey(streamName))
                return Task.FromResult<EventStream>(null);
            var stream = Streams[streamName];
            return Task.FromResult(EventStream.FromEnumerator(stream.Skip(startVersion).Take(untilVersion ?? 1000000).GetEnumerator()));
        }
    }
}