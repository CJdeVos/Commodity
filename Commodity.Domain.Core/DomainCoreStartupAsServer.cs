using Commodity.Interfaces;
using Ninject;
using System;
using Commodity.Common;
using System.Linq;
using Commodity.Domain.Core.Interfaces;

namespace Commodity.Domain.Core
{
    public class DomainCoreStartupAsServer : IStartUpAsServer
    {
        private readonly IKernel _kernel;
        
        public DomainCoreStartupAsServer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Start()
        {
            // create bus
            _kernel.Bind<IEventStorage>().To<InMemoryEventStorage>();
            _kernel.Bind<IEventStore>().To<EventStore>();
            _kernel.Bind<IEventPublisher>().To<EventPublisher>();

            // find all event handlers and bind to self.
            var allEventHandlers = AppDomain.CurrentDomain.GetAssemblies().FindTypesImplementingInterface(typeof(IEventHandler<>));
            foreach (var eventHandlerType in allEventHandlers)
            {
                var interfaces = eventHandlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>)).ToArray();
                _kernel.Bind(interfaces).To(eventHandlerType);
            }
        }

        public void Stop()
        {
        }
    }
}
