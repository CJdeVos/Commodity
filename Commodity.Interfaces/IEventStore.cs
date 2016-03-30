using System;
using System.Security.Cryptography.X509Certificates;

namespace Commodity.Interfaces
{
    public interface IAggregateRootId
    {
        Guid TechnicalId { get; }
    }

    public interface IAggregateRoot
    {
        IAggregateRootId Id { get; }
    }


    public interface IEventStore
    {
        TAggregateRoot Load<TAggregateRoot>(IAggregateRootId aggregateRootId) where TAggregateRoot: IAggregateRoot;
        TAggregateRoot LoadNew<TAggregateRoot>() where TAggregateRoot : IAggregateRoot;
        void Persist<TAggregateRoot>(TAggregateRoot aggregateRoot) where TAggregateRoot : IAggregateRoot;
    }
}
