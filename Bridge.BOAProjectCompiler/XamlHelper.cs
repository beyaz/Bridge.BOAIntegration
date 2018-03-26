using System.Linq;
using System.Xml;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    public static class XamlHelper
    {
        static string GetNamespaceName(XmlNode xamlNode)
        {
            return xamlNode.NamespaceURI.Split(';').FirstOrDefault(x => x != null && x.Contains("clr-namespace:"))?.RemoveFromStart("clr-namespace:");
        }

        public static string GetClassFullName(XmlNode xamlNode)
        {
            return GetNamespaceName(xamlNode) + "." + xamlNode.LocalName;
        }
    }
}