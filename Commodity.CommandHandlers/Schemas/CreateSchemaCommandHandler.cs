using System;
using Commodity.Commands.Schemas;
using Commodity.Interfaces;
using Commodity.Domain.Schemas;

namespace Commodity.CommandHandlers.Schemas
{
    public class CreateSchemaCommandHandler : ICommandHandler<CreateSchemaCommand>
    {
        private readonly IEventStore _eventStore;
        public CreateSchemaCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Handle(CreateSchemaCommand command)
        {
            var schema = _eventStore.LoadNew<Schema>();
            schema.AddProperty("A");
            schema.AddProperty("BC");
            _eventStore.Persist(schema);


        }
    }
}
