using Commodity.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Commodity.Domain.Core.Interfaces
{
    public interface IEventStore
    {
        Task<EventStream> GetEventStream(string streamName, int startVersion, int? untilVersion);
        Task AppendToEventStream(string streamName, int expectedVersion, IEnumerable<IAggregateEvent> events);
    }
}
