using Commodity.Interfaces;

namespace Commodity.Commands.Schemas
{
    public class CreateSchemaCommand : ICommand
    {
        public string Name { get; set; }
        public string[] Properties { get; set; }
    }
}
