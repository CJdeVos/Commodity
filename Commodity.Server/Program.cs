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
using Commodity.Interfaces;
using Ninject;
using Commodity.ProjectionBuilders;

namespace Commodity.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Commodity.Command.Core.CommandCoreStartupAsClient cc = new CommandCoreStartupAsClient(null);
            Commodity.Domain.Core.DomainCoreStartupAsServer dc = new DomainCoreStartupAsServer(null);
            Commodity.CommandHandlers.Schemas.CreateSchemaCommandHandler ch = new CreateSchemaCommandHandler(null);
            SchemaNameAndCountOfProperties sncop = new SchemaNameAndCountOfProperties();
            Schema schema = new Schema(null);



            // kernel for binding
            var kernel = new StandardKernel();
            kernel.BindStartUp<IStartUpAsServer>();
            foreach (var server in kernel.GetAll<IStartUpAsServer>())
            {
                server.Start();
            }

            Console.WriteLine("Wait here. Press a <key> to exit.");
            Console.ReadLine();
            
            
        }
    }
}
