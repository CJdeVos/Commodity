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

        public async void Handle(CreateSchemaCommand command)
        {


            Guid gSchemaId = new Guid("42E5B6A3-0B76-4FE7-B780-71935086B8ED");
            var schema = new Schema(gSchemaId); // = _repository.LoadNew<Schema>();
            schema.AddProperty("A");
            schema.AddProperty("BC");
            _repository.Save(schema);


            // query event store for events
            var storedSchema = await _repository.Load<Schema>(gSchemaId, Int32.MaxValue);
            storedSchema.AddProperty("CD");
            _repository.Save(storedSchema);

        }
    }
}
