using System.Linq;
using System.Xml;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    public static class XamlHelper
    {
        #region Public Methods
        public static string GetCSharpClassFullName(XmlNode xamlNode)
        {
            return GetCSharpNamespaceName(xamlNode) + "." + xamlNode.LocalName;
        }
        #endregion

        #region Methods
        static string GetCSharpNamespaceName(XmlNode xamlNode)
        {
            return xamlNode.NamespaceURI.Split(';').FirstOrDefault(x => x != null && x.Contains("clr-namespace:"))?.RemoveFromStart("clr-namespace:");
        }
        #endregion
    }
}