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

        static readonly object DotnetInstanceCache = ObjectLiteral.Create<object>();

        internal static void Connect_Typescript_And_Dotnet_Instances_for_BrowsePage(object typescriptWrittenInstance,Type bridgeWrittenType)
        {
            if (bridgeWrittenType == null)
            {
                throw new ArgumentNullException(nameof(bridgeWrittenType));
            }

            var pageId = typescriptWrittenInstance[AttributeName.pageId].As<int>().ToString();

            var dotnetInstance = DotnetInstanceCache[pageId];
            if (dotnetInstance == null)
            {
                dotnetInstance = Activator.CreateInstance(bridgeWrittenType);

                DotnetInstanceCache[pageId] = dotnetInstance;
            }

            // TODO: clear memory formun close eventinde burası   DotnetInstanceCache[pageId] delete edilmelidir. 

            typescriptWrittenInstance[DotNetVersion] = dotnetInstance;
            dotnetInstance[TypeScriptVersion] = typescriptWrittenInstance;
            typescriptWrittenInstance[AttributeName.state][AttributeName.columns]= dotnetInstance[AttributeName.DolarColumns];
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