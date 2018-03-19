using System;
using System.Xml;

namespace Bridge.BOAProjectCompiler
{
    class BoaXamlToBoaOneXmlConverter
    {
        #region Public Properties
        public string InputXamlString { get; set; }

        public string OutputXmlString
        {
            get { return RootNode.OuterXml; }
        }

        public XmlNode RootNode { get; set; }
        #endregion

        #region Properties
        XmlDocument Document => RootNode.OwnerDocument;
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

            TransformStackPanel();
        }

        void OnStackPanel(XmlNode xmlNode)
        {
            var bGridSection = Document.CreateElement("BGridSection");

            var nodes = xmlNode.ChildNodes;

            foreach (XmlNode node in nodes)
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
            var elements = Document.SelectNodes("//StackPanel");

            if (elements == null)
            {
                return;
            }

            foreach (XmlNode xmlNode in elements)
            {
                OnStackPanel(xmlNode);
            }
        }
        #endregion
    }
}