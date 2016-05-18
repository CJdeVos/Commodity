using System;
using System.Threading.Tasks;

namespace Commodity.Domain.Core.Interfaces
{
    public interface IAggregateRepository
    {
        Task<TAggregate> Load<TAggregate>(Guid aggregateId, int version) where TAggregate : Aggregate;
        Task Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate;
    }
}
