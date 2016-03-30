namespace Commodity.Interfaces
{
    public interface ICommandHandler<TCommand>
        where TCommand: ICommand
    {
        void Handle(TCommand command);
    }
}
