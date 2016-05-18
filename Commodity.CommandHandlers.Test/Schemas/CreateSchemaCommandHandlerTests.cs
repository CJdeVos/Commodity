using System.Threading.Tasks;
using Commodity.CommandHandlers.Schemas;
using Commodity.Commands.Schemas;
using Commodity.Domain.Schemas;
using Commodity.Domain.Schemas.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Commodity.CommandHandlers.Test.Schemas
{
    [TestClass]
    public class CreateSchemaCommandHandlerTests : AggregateRepositoryTest
    {
        private CreateSchemaCommandHandler _commandHandler;

        [TestInitialize]
        public void Initialize()
        {
            _commandHandler = new CreateSchemaCommandHandler(AggregateRepository);
        }


        [TestMethod]
        public void CreateNewSchema()
        {
            // Arrange
            const string schemaName = "Test Schema";
            string[] schemaProperties = new string[] {"Aap", "Noot", "Mies "};
            
            // Act
            var command = new CreateSchemaCommand()
            {
                Name = schemaName,
                Properties = schemaProperties
            };

            Should.NotThrow(async () => await _commandHandler.Handle(command));

            AssertEventsRecorded(
                (@event) => @event.Is<SchemaRenamed>(schemaRenamed => schemaRenamed.SchemaName == schemaName),
                (@event) => @event.Is<SchemaPropertyCreated>(schemaPropertyCreated => schemaPropertyCreated.PropertyName == schemaProperties[0]),
                (@event) => @event.Is<SchemaPropertyCreated>(schemaPropertyCreated => schemaPropertyCreated.PropertyName == schemaProperties[1]),
                (@event) => @event.Is<SchemaPropertyCreated>(schemaPropertyCreated => schemaPropertyCreated.PropertyName == schemaProperties[2])
            );
        }
    }
}
