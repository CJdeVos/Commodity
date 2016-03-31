﻿using Commodity.Interfaces;

namespace Commodity.Domain.Core.Events
{
    public class Created<TAggregate> : IAggregateEvent
        where TAggregate: Aggregate
    {
    }
}
