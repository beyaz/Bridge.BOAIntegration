using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace BOA.Common.Helpers
{
    /// <summary>
    ///     The XML helper
    /// </summary>
    public static class XmlHelper
    {
        #region Public Methods
        /// <summary>
        ///     Clears the XML.
        /// </summary>
        public static string ClearXml(string xml)
        {
            var stringBuilder = new StringBuilder();

            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
                //Indent = false,
                //NewLineOnAttributes = false
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Pretties the XML.
        /// </summary>
        public static string PrettyXml(string xml)
        {
            var stringBuilder = new StringBuilder();

            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration  = true,
                Indent              = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     To the list.
        /// </summary>
        public static List<XmlNode> ToList(this IEnumerable nodeList)
        {
            var xmlNodes = new List<XmlNode>();

            foreach (XmlNode xmlNode in nodeList)
            {
                xmlNodes.Add(xmlNode);
            }

            return xmlNodes;
        }
        #endregion
    }
}