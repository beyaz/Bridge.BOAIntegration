using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bridge.BOAIntegration
{
    class Utility
    {
        public static T ConvertToBridgeGeneratedType<T>(object jsonValue)
        {
            var jsonString = JSON.Stringify(jsonValue);

            return JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
