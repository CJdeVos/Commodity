using System;

namespace Commodity.Domain.Core.Interfaces
{
    public interface IEventStore
    {
        TAggregate Load<TAggregate>(Guid aggregateId) where TAggregate: Aggregate;
        void Persist<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;
    }
}
