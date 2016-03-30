using System;
using Commodity.Interfaces;

namespace Commodity.Domain.Core
{
    public class AggregateRootId<TAggregateRoot> : IAggregateRootId
        where TAggregateRoot: IAggregateRoot
    {
        public AggregateRootId(Guid id)
        {
            TechnicalId = id;
        }

        public Guid TechnicalId { get; private set; }
    }
}
