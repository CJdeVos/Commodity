using Commodity.Interfaces;

namespace Commodity.Domain.Core.Events
{
    public class Created<T> : IAggregateEvent
        where T: IAggregateRoot
    {
    }
}
