using System;
using Bridge.Html5;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bridge.BOAIntegration
{
    static class Utility
    {
        const string  DotNetVersion = "$DotNetVersion";

        const string TypeScriptVersion = "$TypeScriptVersion";

        internal static void Connect_Typescript_And_Dotnet_Instances_for_BrowsePage(object typescriptWrittenInstance,Type bridgeWrittenType)
        {
            if (bridgeWrittenType == null)
            {
                throw new ArgumentNullException(nameof(bridgeWrittenType));
            }

            typescriptWrittenInstance[DotNetVersion] = Activator.CreateInstance(bridgeWrittenType);
            typescriptWrittenInstance[DotNetVersion][TypeScriptVersion] = typescriptWrittenInstance;
            typescriptWrittenInstance[AttributeName.state][AttributeName.columns]= typescriptWrittenInstance[DotNetVersion][AttributeName.DolarColumns];
        }

        #region Public Methods
        public static object ConvertDotnetInstanceToBOAJsonObject(object dotnetInstance)
        {
            var jsonString = JsonConvert.SerializeObject(dotnetInstance, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });

            return JSON.Parse(jsonString);
        }

        public static T ConvertBOAJsonObjectToDotnetInstance<T>(object jsonValue)
        {
            return JsonConvert.DeserializeObject<T>(jsonValue.As<string>(), new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }

        public static object ConvertBOAJsonObjectToDotnetInstance(object jsonValue,Type targetType)
        {
            return JsonConvert.DeserializeObject(jsonValue.As<string>(), targetType, new JsonSerializerSettings
            {
                ContractResolver     = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }
        #endregion
    }
}