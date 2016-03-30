using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Commodity.Commands.Schemas;
using Commodity.Interfaces;

namespace FrontEnd.Api
{
    public class SchemaController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public SchemaController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new[] {"A", "B"};
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post(CreateSchemaCommand cmd)
        {
            _commandDispatcher.Dispatch(cmd);
            //return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created));
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}
