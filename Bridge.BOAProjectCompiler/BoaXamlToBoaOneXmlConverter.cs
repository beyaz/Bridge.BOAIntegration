using System;
using System.Collections.Generic;
using System.Linq;
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
                    _namespaces = XmlHelper.GetAllNamespaces(Document);
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

        string boa_ui_ns => Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI;assembly=BOA.UI").Key;
        


        void ApplyComponentTransforms()
        {
            Transform_BMaskedEditorLabeled();
        }
        void Transform_BMaskedEditorLabeled()
        {
            if (boa_ui_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_ui_ns + ":" + "BMaskedEditorLabeled").ToList().ForEach(Transform_BMaskedEditorLabeled);
        }

        
        void Transform_BMaskedEditorLabeled(XmlNode node)
        {
            var newElement = Document.CreateElement("BInputMask");

            var maskValueIsEqualToCardNumber = node.Attributes?["Mask"]?.Value == "#### #### #### ####";
            if (maskValueIsEqualToCardNumber)
            {
                newElement.SetAttribute("type", "CreditCard");
            }
            else if (node.Attributes?["Mask"] != null)
            {
                newElement.SetAttribute("type", "custom");
                newElement.SetAttribute("mask", node.Attributes?["Mask"]?.Value);
            }

            
            if (node.Attributes?["Text"] != null)
            {
                newElement.SetAttribute("value", node.Attributes?["Text"]?.Value);
            }

            if (node.Attributes?["Label"] != null)
            {
                newElement.SetAttribute("hintText", node.Attributes?["Label"]?.Value);
                newElement.SetAttribute("floatingLabelText", node.Attributes?["Label"]?.Value);
            }

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
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