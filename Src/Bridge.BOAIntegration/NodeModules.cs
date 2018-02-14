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

        static string FindComponentExportName(string nodeTagName)
        {
            if ("BINPUTMASK" == nodeTagName)
            {
                return "BInputMask";
            }

            if ("BDATETIMEPICKER" == nodeTagName)
            {
                return "BDateTimePicker";
            }


            if ("BCOMBOBOX" == nodeTagName)
            {
                return "BComboBox";
            }

            

            return nodeTagName;
        }
        internal static object FindComponent(string nodeTagName)
        {
            var key = FindComponentExportName(nodeTagName);

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
                    if (i<2000)
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
                if (componentConstructor.As<bool>())
                {
                    _cache[key] = componentConstructor;

                    return componentConstructor;

                }


                // TODO: buraya gelmiyor olablir
                var defaultt = exports["default"];
                if (defaultt == null)
                {
                    continue;
                }

                string name = defaultt["name"].As<string>();
                if (!name.As<bool>())
                {
                    continue;
                }

                if (key == name.ToUpper())
                {
                    _cache[key] = defaultt;

                    return defaultt;
                }
            }
        }
        #endregion
    }
}