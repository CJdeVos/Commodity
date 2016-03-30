using Commodity.Interfaces;
using MassTransit;
using Ninject;
using System;
using MassTransit.Log4NetIntegration.Logging;

namespace Commodity.Command.Core
{
    public class CommandCoreStartupAsClient : IStartUpAsClient
    {
        private readonly IKernel _kernel;
        private BusHandle _handle;
        public CommandCoreStartupAsClient(IKernel kernel)
        {
            _kernel = kernel;
            
        }

        public void Start()
        {
            _kernel.Bind<ICommandDispatcher>().To<CommandDispatcher>();
            BindBusControlRabbitMq();
        }

        public void Stop()
        {
            _handle.Stop();
        }

        private void BindBusControlRabbitMq()
        {
            Log4NetLogger.Use();
            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
              x.Host(new Uri("rabbitmq://localhost/"), h => { }));
            _handle = bus.Start();

            _kernel.Bind<IBusControl>().ToConstant(bus);
        }
    }
}
