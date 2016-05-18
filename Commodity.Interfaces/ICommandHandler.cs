using System.Threading.Tasks;

namespace Commodity.Interfaces
{
    public interface ICommandHandler<TCommand>
        where TCommand: ICommand
    {
        Task Handle(TCommand command);
    }
}
