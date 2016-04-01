using System;

namespace Commodity.Domain.Core.Interfaces
{
    public interface IEventStreamNameResolver
    {
        string Resolve<TAggregate>(Guid aggregateId) where TAggregate : Aggregate;
    }
}
