using Commodity.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace Commodity.Command.Core
{
    public class CommandConsumer<TCommand, TCommandHandler> : IConsumer<TCommand> 
        where TCommand: class, ICommand
        where TCommandHandler: ICommandHandler<TCommand>
    {
        private readonly TCommandHandler _commandHandler;
        public CommandConsumer(TCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public Task Consume(ConsumeContext<TCommand> context)
        {
            _commandHandler.Handle(context.Message);
            return Task.FromResult(0x0);
        }
    }
}
