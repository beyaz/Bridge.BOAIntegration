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
        string      boa_ui_ns => Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI;assembly=BOA.UI").Key;
        string boa_BusinessComponents_ns => Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI.BusinessComponents;assembly=BOA.UI.BusinessComponents").Key;
        XmlDocument Document  => RootNode.OwnerDocument;

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

        static void TransferAttribute(XmlNode xamlNode, string xamlPropertyName, XmlElement newElement, string newAttribute)
        {
            if (xamlNode.Attributes?[xamlPropertyName] != null)
            {
                newElement.SetAttribute(newAttribute, xamlNode.Attributes?[xamlPropertyName]?.Value);
            }
        }

        static void TransferNameAttribute(XmlNode xamlNode, XmlElement newElement)
        {
            TransferAttribute(xamlNode, "x:Name", newElement, "x.Name");
        }

        void ApplyComponentTransforms()
        {
            Transform_BMaskedEditorLabeled();
            Transform_BDateTimeEditorLabeled();
            Transform_AccountComponent();
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

        void Transform_BDateTimeEditorLabeled(XmlNode node)
        {
            var newElement = Document.CreateElement("BDateTimePicker");

            TransferAttribute(node, "Value", newElement, "value");
            TransferAttribute(node, "Label", newElement, "floatingLabelTextDate");
            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        void Transform_BMaskedEditorLabeled()
        {
            if (boa_ui_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_ui_ns + ":" + "BMaskedEditorLabeled").ToList().ForEach(Transform_BMaskedEditorLabeled);
        }

        void Transform_BDateTimeEditorLabeled()
        {
            if (boa_ui_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_ui_ns + ":" + "BDateTimeEditorLabeled").ToList().ForEach(Transform_BMaskedEditorLabeled);
        }

        void Transform_AccountComponent()
        {
            if (boa_BusinessComponents_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_BusinessComponents_ns + ":" + "AccountComponent").ToList().ForEach(Transform_AccountComponent);
        }

        void Transform_AccountComponent(XmlNode node)
        {
            var newElement = Document.CreateElement("BAccountComponent");

           

            TransferAttribute(node, "AccountNumber", newElement, "accountNumber");

            if (node.Attributes?["VisibilityAccountSuffix"]?.Value == "Collapsed")
            {
                newElement.SetAttribute("isVisibleAccountSuffix", "false");
            }

            TransferAttribute(node, "ShowTaxNumberAndMernisVerifiedDialogMessage", newElement, "showTaxNumberAndMernisVerifiedDialogMessage");
            TransferAttribute(node, "ShowMernisServiceHealtyDialogMessage", newElement, "showMernisServiceHealtyDialogMessage");
            TransferAttribute(node, "ShowDialogMessages", newElement, "showDialogMessages");
            TransferAttribute(node, "ShowCustomerRecordingBranchWarning", newElement, "showCustomerRecordingBranchWarning");
            TransferAttribute(node, "ShowCustomerBranchAccountMessage", newElement, "showCustomerBranchAccountMessage");
            TransferAttribute(node, "ShowBlackListDialogMessages", newElement, "showBlackListDialogMessages");
            TransferAttribute(node, "AllowSharedAccountControl", newElement, "allowSharedAccountControl");
            TransferAttribute(node, "AllowDoubleSignatureControl", newElement, "allowDoubleSignatureControl");
            TransferAttribute(node, "Allow18AgeControl", newElement, "allow18AgeControl"); 




            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
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
                newElement.SetAttribute("type", "Custom");
                newElement.SetAttribute("mask", node.Attributes?["Mask"]?.Value?.Replace("#", "n"));
            }

            TransferAttribute(node, "Text", newElement, "value");
            TransferAttribute(node, "Label", newElement, "hintText");
            TransferAttribute(node, "Label", newElement, "floatingLabelText");

            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
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