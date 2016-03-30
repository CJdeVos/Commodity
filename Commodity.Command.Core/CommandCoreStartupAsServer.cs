using Commodity.Interfaces;
using MassTransit;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit.Log4NetIntegration.Logging;
using Commodity.Common;

namespace Commodity.Command.Core
{
    public class CommandCoreStartupAsServer : IStartUpAsServer
    {
        private readonly IKernel _kernel;
        private BusHandle _handle;

        public CommandCoreStartupAsServer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Start()
        {
            // create bus
            BindBusControlRabbitMq();
        }

        public void Stop()
        {
            _handle.Stop();
        }


        private void BindBusControlRabbitMq()
        {
            // Find command handlers in all assemblies.
            var allCommandHandlers = AppDomain.CurrentDomain.GetAssemblies().FindTypesImplementingInterface(typeof(ICommandHandler<>));

            // Map generic type of ICommandHandler with each actual command handler.
            Dictionary<Type, Type> dictOfArgumentAndCommandHandler = new Dictionary<Type, Type>();
            foreach (Type commandHandlerType in allCommandHandlers)
            {
                var argumentOfCommandHandlerType =
                    commandHandlerType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICommandHandler<>)).GetGenericArguments().First();

                if(dictOfArgumentAndCommandHandler.ContainsKey(argumentOfCommandHandlerType))
                    throw new Exception("Only 1 command handler allowed per command.");

                // Consume handler
                Type consumeHandlerType = typeof (CommandConsumer<,>).MakeGenericType(argumentOfCommandHandlerType, commandHandlerType);


                // add it for mapping purposes
                dictOfArgumentAndCommandHandler.Add(argumentOfCommandHandlerType, consumeHandlerType);

                // and add it to kernel
                _kernel.Bind(commandHandlerType).ToSelf();
                _kernel.Bind(consumeHandlerType).ToSelf();
            }

            // standard initialization
            Log4NetLogger.Use();

            
            var bus = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(new Uri("rabbitmq://localhost/"), h => { });

                x.ReceiveEndpoint(host, "CommodityAllMessages", e => {
                    // create consumer for every single command handler -> 
                    foreach (Type commandHandlerType in dictOfArgumentAndCommandHandler.Values) {

                        e.Consumer(commandHandlerType, (t) =>
                        {
                            return _kernel.Get(t);
                        });                                                         
                    }

                        //e.Consumer<CommandConsumer>(() => _kernel.Get<CommandConsumer>());
                });
            });
            _kernel.Bind<IBusControl>().ToConstant(bus);

            _handle = bus.Start();
        }
    }
}
