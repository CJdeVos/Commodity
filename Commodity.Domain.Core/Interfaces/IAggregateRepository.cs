using System;

namespace Commodity.Domain.Core.Interfaces
{
    public interface IAggregateRepository
    {
        TAggregate Load<TAggregate>(Guid aggregateId, int version) where TAggregate : Aggregate;
        void Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;
    }
}
