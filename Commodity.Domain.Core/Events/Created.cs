using Commodity.Interfaces;
using Commodity.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Commodity.Domain.Core.Events
{
    [CommoditySerializable("Created")]
    public class Created<TAggregate> : IAggregateEvent
        where TAggregate: Aggregate
    {
    }
}
