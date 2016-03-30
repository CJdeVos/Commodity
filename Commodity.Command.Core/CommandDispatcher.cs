using System;
using Commodity.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace Commodity.Command.Core
{
    // Core Command Dispatcher dispatches the commands to RabbitMQ
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IBusControl _bus;

        public CommandDispatcher(IBusControl bus)
        {
            _bus = bus;
        }

        public async Task Dispatch<TCommand>(TCommand command) where TCommand: class, ICommand
        {
            await _bus.Publish(command);
        }
    }
}
