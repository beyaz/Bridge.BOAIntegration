using System.Collections.Generic;
using System.Linq;

namespace Bridge.BOAProjectCompiler
{
    static class BOAXamlHelper
    {
        #region Public Methods
        public static string GetNamespacePrefix_boa_BusinessComponents_ns(IDictionary<string, string> Namespaces)
        {
            return Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI.BusinessComponents;assembly=BOA.UI.BusinessComponents").Key;
        }

        public static string GetNamespacePrefix_boa_ui_ns(IDictionary<string, string> Namespaces)
        {
            return Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI;assembly=BOA.UI").Key;
        }
        #endregion
    }
}