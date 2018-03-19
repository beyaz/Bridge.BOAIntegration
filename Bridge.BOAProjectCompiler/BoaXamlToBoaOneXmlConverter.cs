using System;
using System.Collections.Generic;
using System.Xml;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    class BoaXamlToBoaOneXmlConverter
    {
        #region Fields
        IDictionary<string, string> _namespaces;
        #endregion

        #region Public Properties
        public string InputXamlString { get; set; }

        public string OutputXmlString
        {
            get { return XmlHelper.PrettyXml(RootNode.OuterXml); }
        }

        public XmlNode RootNode { get; set; }
        #endregion

        #region Properties
        XmlDocument Document => RootNode.OwnerDocument;

        IDictionary<string, string> Namespaces
        {
            get
            {
                if (_namespaces == null)
                {
                    var nsMgr = new XmlNamespaceManager(Document.NameTable);

                    _namespaces = nsMgr.GetNamespacesInScope(XmlNamespaceScope.All);
                }

                return _namespaces;
            }
        }
        #endregion

        #region Methods
        internal void TransformNodes()
        {
            if (RootNode == null)
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(InputXamlString);
                RootNode = xmlDocument.FirstChild;
            }

            ApplyLayoutTransforms();

            ApplyComponentTransforms();
        }

        void ApplyComponentTransforms()
        {
            Document.GetElementsByTagName("BMaskedEditorLabeled");
        }

        void ApplyLayoutTransforms()
        {
            TransformStackPanel();
        }

        void OnStackPanel(XmlNode xmlNode)
        {
            var bGridSection = Document.CreateElement("BGridSection");

            var nodes = xmlNode.ChildNodes.ToList();

            foreach (var node in nodes)
            {
                var bGridRow = Document.CreateElement("BGridRow");
                if (node.ParentNode == null)
                {
                    throw new ArgumentException();
                }

                node.ParentNode.RemoveChild(node);

                bGridRow.AppendChild(node);

                bGridSection.AppendChild(bGridRow);
            }

            if (xmlNode.ParentNode == null)
            {
                throw new ArgumentException();
            }

            xmlNode.ParentNode.InsertBefore(bGridSection, xmlNode);

            xmlNode.ParentNode.RemoveChild(xmlNode);
        }

        void TransformStackPanel()
        {
            var nodeList = Document.GetElementsByTagName("StackPanel").ToList();

            foreach (var xmlNode in nodeList)
            {
                OnStackPanel(xmlNode);
            }
        }
        #endregion
    }
}