using Commodity.Interfaces;

namespace Commodity.Domain.Core.Events
{
    [CommodityBsonSerializable("Created")]
    public class Created<TAggregate> : IAggregateEvent
        where TAggregate: Aggregate
    {
    }
}
