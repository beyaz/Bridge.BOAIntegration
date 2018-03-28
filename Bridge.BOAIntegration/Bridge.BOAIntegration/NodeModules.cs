using System;
using Bridge.Html5;

namespace Bridge.BOAIntegration
{
    static class JsLocation
    {
        #region Properties
        internal static object __webpack_require__ => Script.Write<object>("Bridge.BOAIntegration.$__webpack_require__");

        internal static Function _extend => Script.Write<Function>("Bridge.$BOAIntegration.$_extends");
        #endregion
    }


   


    public static class NodeModules
    {
        #region Static Fields
        static readonly object _cache = new object();
        #endregion

        #region Public Methods
        public static Function BDialogHelper()
        {
            return FindByExportKey(nameof(BDialogHelper)).As<Function>();
        }
        public static Function BFormManager()
        {
            return FindByExportKey(nameof(BFormManager)).As<Function>();
        }
        

        public static Function getMessage()
        {
            return FindByExportKey(nameof(getMessage)).As<Function>();
        }
        #endregion

        #region Methods
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
                var module = nodeModuleCache[i.ToString()];

                if (module == null)
                {
                    if (i < ExpectedEntryCount)
                    {
                        continue;
                    }

                    return null;
                }

                var exports = module["exports"];
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

        const int ExpectedEntryCount = 2100;

        internal static object FindComponent(string nodeTagName)
        {
            var key = nodeTagName;

            var value = _cache[key];
            if (value != null)
            {
                return value;
            }

            var nodeModuleCache = JsLocation.__webpack_require__["c"];

            var i = -1;

            while (true)
            {
                i++;
                var module = nodeModuleCache[i.ToString()];

                if (module == null)
                {
                    if (i < ExpectedEntryCount)
                    {
                        continue;
                    }

                    return null;
                }

                var exports = module["exports"];
                if (exports == null)
                {
                    continue;
                }

                var componentConstructor = exports[key];
                if (componentConstructor != Script.Undefined)
                {
                    _cache[key] = componentConstructor;

                    return componentConstructor;
                }
            }
        }
        #endregion
    }
}