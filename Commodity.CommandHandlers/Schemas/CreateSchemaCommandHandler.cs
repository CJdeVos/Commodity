using System;
using Commodity.Commands.Schemas;
using Commodity.Domain.Core.Interfaces;
using Commodity.Interfaces;
using Commodity.Domain.Schemas;
using System.Threading.Tasks;

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

        public async Task Handle(CreateSchemaCommand command)
        {
            // Define id for the new schema (assume the id doesn't exist yet)
            Guid gSchemaId = Guid.NewGuid();

            // create schema
            var schema = new Schema(gSchemaId);

            // set name
            schema.Rename(command.Name);

            // add properties
            foreach(var property in command.Properties)
                schema.AddProperty(property);

            // save
            await _repository.Save(schema);
        }

        public async void HandleXX(CreateSchemaCommand command)
        {
            Guid gSchemaId = new Guid("42E5B6A3-0B76-4FE7-B780-71935086B8ED");
            var storedSchemaX = await _repository.Load<Schema>(gSchemaId, Int32.MaxValue);
            if (storedSchemaX == null)
            {
                Console.WriteLine("Item is not found. Create a new one.");
            }

            var schema = new Schema(gSchemaId); // = _repository.LoadNew<Schema>();
            schema.AddProperty("A");
            schema.AddProperty("BC");
            await _repository.Save(schema);


            // query event store for events
            var storedSchema = await _repository.Load<Schema>(gSchemaId, Int32.MaxValue);
            if (storedSchema == null)
            {
                Console.WriteLine("Item STILL not found.");
            }
            else
            {
                storedSchema.AddProperty("CD");
                await _repository.Save(storedSchema);
            }
            
        }
    }
}
