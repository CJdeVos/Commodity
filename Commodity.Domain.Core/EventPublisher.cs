using System;
using System.Collections.Generic;
using Commodity.Domain.Core.Interfaces;
using Commodity.Interfaces;
using Ninject;

namespace Commodity.Domain.Core
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IKernel _kernel;
        public EventPublisher(IKernel kernel)
        {
            _kernel = kernel;
        }

        //public void Publish<TAggregateId>(TAggregateId aggregate, IEnumerable<IAggregateEvent> events)
        //    where TAggregateId: IEquatable<TAggregateId>
        //{
        //    foreach(var e in events)
        //    {
        //        Type eventHandlerType = typeof(IHandleEvent<,>);
        //        Type genericEventHandlerType = eventHandlerType.MakeGenericType(typeof(TAggregateId), e.GetType());
        //        // find all internal events
        //        var g = _kernel.GetAll(genericEventHandlerType);
        //        foreach (dynamic d in g){
        //            d.Handle(aggregate, (dynamic)e);
        //        }
        //    }
        //}

        public void Publish(IAggregateRootId aggregateRootId, IEnumerable<IAggregateEvent> events)
        {
            Type eventContextType = typeof(EventContext<>);
            foreach (var e in events)
            {
                Type eventHandlerType = typeof(IEventHandler<>);
                Type eventContextForEventType = eventContextType.MakeGenericType(e.GetType());
                Type genericEventHandlerType = eventHandlerType.MakeGenericType(eventContextForEventType);
                var g = _kernel.GetAll(genericEventHandlerType);
                foreach (dynamic d in g)
                {
                    eventContextForEventType.GetConstructor()
                    d.Handle(aggregateRootId, (dynamic)e);
                }
            }
        }
    }
}
