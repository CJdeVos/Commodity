using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commodity.Command.Core;
using Commodity.CommandHandlers.Schemas;
using Commodity.Common;
using Commodity.Domain;
using Commodity.Domain.Core;
using Commodity.Domain.Schemas;
using Commodity.Domain.Schemas.Events;
using Commodity.Interfaces;
using Ninject;
using Commodity.ProjectionBuilders;
using Commodity.Serialization;
using Commodity.Serialization.Serializers;

namespace Commodity.Server
{
  

    class Program
    {
        static void Main(string[] args)
        {
            Commodity.Command.Core.CommandCoreStartupAsClient cc = new CommandCoreStartupAsClient(null);
            Commodity.Domain.Core.StartupAsServer dc = new StartupAsServer(null);
            Commodity.CommandHandlers.Schemas.CreateSchemaCommandHandler ch = new CreateSchemaCommandHandler(null);
            SchemaNameAndCountOfProperties sncop = new SchemaNameAndCountOfProperties();
            SchemaRenamed schemaRn = new SchemaRenamed();
            Commodity.Serialization.Serializers.StringSerializer r = new StringSerializer();



            // kernel for binding
            var kernel = new StandardKernel();
            kernel.BindStartUp<IStartUpAsServer>();
            var allServers = kernel.GetAll<IStartUpAsServer>();
            foreach (var server in allServers)
            {
                server.Start();
            }

            Console.WriteLine("Wait here. Press a <key> to exit.");
            Console.ReadLine();
            
            
        }
    }
}
