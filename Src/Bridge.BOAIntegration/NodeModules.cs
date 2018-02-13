using System;
using Bridge.Html5;

namespace Bridge.BOAIntegration
{
    static class JsLocation
    {
        internal static object __webpack_require__ => Script.Write<object>("Bridge.BOAIntegration.$__webpack_require__");
    }


    public static class NodeModules
    {
        static readonly object _cache = new object();

        public static Function getMessage()
        {
            return FindByExportKey(nameof(getMessage)).As<Function>();
        }


        internal static object FindByExportKey(string exportKey)
        {
            
            var value = _cache[exportKey];
            if (value != null)
            {
                return value;
            }


            var nodeModuleCache = JsLocation.__webpack_require__["c"];


            var i = -1;

            while (true)
            {
                i++;
                dynamic module = nodeModuleCache[i.ToString()];

                if (module == null)
                {
                    return null;
                }

                var exports = module.exports;
                if (exports == null)
                {
                    continue;
                }

                var result = exports[exportKey];
                if (result != null)
                {
                    _cache[exportKey] = result;
                    return result;
                }
                
            }
        }
    }
}