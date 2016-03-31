using Commodity.Interfaces;

namespace Commodity.Domain.Core.Events
{
    public class Deleted<TAggregate> : IAggregateEvent
        where TAggregate: Aggregate
    {
    }
}
