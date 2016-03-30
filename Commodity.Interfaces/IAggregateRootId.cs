using System;

namespace Commodity.Interfaces
{
    public interface IAggregateRootId
    {
        Guid TechnicalId { get; }
    }
}