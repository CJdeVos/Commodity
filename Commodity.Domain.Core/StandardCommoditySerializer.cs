using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commodity.Domain.Core
{
    public class StandardCommoditySerializer : ICommoditySerializer
    {
        public object Deserialize(ICommodityReader reader)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ICommodityWriter writer, object o)
        {
            throw new NotImplementedException();
        }
    }
}
