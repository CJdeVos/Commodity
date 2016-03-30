using System.Threading.Tasks;

namespace Commodity.Interfaces
{
    public interface ICommandDispatcher
    {
        Task Dispatch<TCommand>(TCommand command) where TCommand: class, ICommand;
    }
}
