using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.BOAIntegration
{
    [ObjectLiteral]
    public class BPageParams
    {
        [Name("resourceInfo")]
        public object ResourceInfo { get; set; }

        [Name("data")]
        public object Data { get; set; }
    }
}
