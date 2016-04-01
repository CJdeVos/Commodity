using System;
using Commodity.Commands.Schemas;
using Commodity.Domain.Core.Interfaces;
using Commodity.Interfaces;
using Commodity.Domain.Schemas;

namespace Commodity.CommandHandlers.Schemas
{
    public class CreateSchemaCommandHandler : 
        ICommandHandler<CreateSchemaCommand>
    {
        private readonly IAggregateRepository _repository;
        public CreateSchemaCommandHandler(IAggregateRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateSchemaCommand command)
        {
            var schema = new Schema(Guid.NewGuid()); // = _repository.LoadNew<Schema>();
            schema.AddProperty("A");
            schema.AddProperty("BC");
            _repository.Save(schema);


            // query event store for events
            _repository.Load<Schema>(Guid.NewGuid(), Int32.MaxValue);

        }
    }
}
