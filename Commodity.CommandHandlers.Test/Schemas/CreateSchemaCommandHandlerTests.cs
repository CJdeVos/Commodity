using System;
using System.Threading.Tasks;
using Commodity.CommandHandlers.Schemas;
using Commodity.Commands.Schemas;
using Commodity.Domain.Core.Interfaces;
using Commodity.Domain.Schemas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Shouldly;

namespace Commodity.CommandHandlers.Test.Schemas
{
    [TestClass]
    public class CreateSchemaCommandHandlerTests
    {
        private CreateSchemaCommandHandler _commandHandler;
        private IAggregateRepository _aggregateRepository;

        [TestInitialize]
        public void Initialize()
        {
            _aggregateRepository = MockRepository.GenerateMock<IAggregateRepository>();
            
            _commandHandler = new CreateSchemaCommandHandler(_aggregateRepository);
        }


        [TestMethod]
        public async Task CreateNewSchema()
        {
            // Arrange
            const string schemaName = "Test Schema";
            string[] schemaProperties = new string[] {"Aap", "Noot", "Mies "};
            _aggregateRepository
                .Expect(r => r.Save(Arg<Schema>.Matches(schema =>
                    schema.Name == schemaName
                    && schema.Properties[0] == "Aap"
                    && schema.CommittedVersion == 0
                    )))
                .Return(Task.FromResult(0));

            // Act
            var command = new CreateSchemaCommand()
            {
                Name = schemaName,
                Properties = schemaProperties
            };

            Should.NotThrow(async () => await _commandHandler.Handle(command));

            //await _commandHandler.Handle(command);
            _aggregateRepository.VerifyAllExpectations();
            
            //try
            //{
            //    await _commandHandler.Handle(command);
            //}
            //catch (Exception ee)
            //{
            //    var v = ee;
            //}
            //finally
            //{
            //    _aggregateRepository.VerifyAllExpectations();
            //}
            //Task.Run(() => _commandHandler.Handle(command)).GetAwaiter().GetResult();
            

            // Assert
            
        }
    }
}
