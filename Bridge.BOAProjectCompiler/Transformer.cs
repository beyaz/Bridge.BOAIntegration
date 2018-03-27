using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using BOA.Common.Helpers;


namespace Bridge.BOAProjectCompiler
{
    public class Transformer
    {
        #region Public Methods
        public static void AccountComponent(TransformerInput input)
        {
            var node     = input.XmlNode;
            var Document = input.Document;

            if (input.boa_BusinessComponents_ns == null || node.Name != input.boa_BusinessComponents_ns + ":" + "AccountComponent")
            {
                return;
            }

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

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        public static void BBranchComponent(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_BusinessComponents_ns == null || node.Name != input.boa_BusinessComponents_ns + ":" + "BranchComponent")
            {
                return;
            }

            var Document = input.Document;

            var newElement = Document.CreateElement("BBranchComponent");

            TransferAttribute(node, "SelectedBranchId", newElement, "selectedBranchId");

            newElement.SetAttribute("mode", "horizontal");

            TransferAttribute(node, "Label", newElement, "labelText");

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }


        public static void BOAControls_BField(TransformerInput input)
        {
            var node = input.XmlNode;

            if (node.Name == "BOAControls:BrowseForm.ControlGridFieldSettings" ||
                node.Name == "BOAControls:BrowseForm.ControlGridFieldLayout")
            {
                node.ParentNode?.RemoveChild(node);
                return;
            }

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BField")
            {
                return;
            }

            if (XamlHelper.GetClassFullName(node?.ParentNode?.ParentNode?.ParentNode) != "BOA.UI.BrowseForm.ControlGridFieldLayout")
            {
                return;
            }

            

            input.InfragisticsDataPresenterFields.Add(node);

            node.ParentNode?.RemoveChild(node);

        }
        public static void BComboEditorLabeled(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BComboEditorLabeled")
            {
                
                return;
            }

            var Document = input.Document;

            var newElement = Document.CreateElement("BComboBox");

            TransferAttribute(node, "Label", newElement, "labelText");
            TransferAttribute(node, "ItemsSource", newElement, "dataSource");
            TransferAttribute(node, "DisplayMemberPath", newElement, "displayMemberPath");
            TransferAttribute(node, "ValuePath", newElement, "valuePath");
            TransferAttribute(node, "Value", newElement, "value");

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }
        public static void BComboEditorMultiSelect(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BComboEditorMultiSelect" )
            {
                return;
            }

            var Document = input.Document;

            var newElement = Document.CreateElement("BComboBox");

            TransferAttribute(node, "Label", newElement, "labelText");
            TransferAttribute(node, "ItemsSource", newElement, "dataSource");
            TransferAttribute(node, "SelectedItems", newElement, "selectedItems");

            newElement.SetAttribute("displayLabelSeperator", ",");
            newElement.SetAttribute("multiSelect", "true");
            newElement.SetAttribute("multiColumn", "true");
            newElement.SetAttribute("isAllOptionIncluded", "true");

            var bfieldLayoutNode = ((XmlElement) node).GetElementsByTagName(input.boa_ui_ns + ":BFieldLayout").ToList().FirstOrDefault();

            if (bfieldLayoutNode != null)
            {
                var columnNodes = bfieldLayoutNode.ChildNodes.ToList().ConvertAll(n =>
                {
                    var comboBoxColumn = Document.CreateElement("ComboBoxColumn");

                    if (n.Attributes?.GetNamedItem("Name")?.Value != null)
                    {
                        comboBoxColumn.SetAttribute("key", n.Attributes?.GetNamedItem("Name")?.Value);
                    }

                    if (n.Attributes?.GetNamedItem("Label")?.Value != null)
                    {
                        comboBoxColumn.SetAttribute("Name", n.Attributes?.GetNamedItem("Label")?.Value);
                    }

                    return comboBoxColumn;
                });

                var BComboBox_Columns = Document.CreateElement("BComboBox.Columns");

                columnNodes.ForEach(x => BComboBox_Columns.AppendChild(x));

                newElement.AppendChild(BComboBox_Columns);
            }

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        public static void BDateTimeEditorLabeled(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BDateTimeEditorLabeled")
            {
                return;
            }

            var Document   = input.Document;
            var newElement = Document.CreateElement("BDateTimePicker");

            newElement.SetAttribute("format", "DDMMYYYY");
            TransferAttribute(node, "Value", newElement, "value");
            TransferAttribute(node, "Label", newElement, "floatingLabelTextDate");
            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }
        public static void BNumericEditorLabeled(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BNumericEditorLabeled")
            {
                return;
            }

            var Document   = input.Document;
            var newElement = Document.CreateElement("BInputNumeric");

            if (node.Attributes?["ValueType"]?.Value == "{x:Type sys:Decimal}")
            {
                newElement.SetAttribute("format", "M");
            }

            
            TransferAttribute(node, "Value", newElement, "value");
            TransferAttribute(node, "Label", newElement, "floatingLabelTextDate");
            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }
        

        public static void BMaskedEditorLabeled(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BMaskedEditorLabeled")
            {
                return;
            }

            var Document = input.Document;

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

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        public static void BTextEditorLabeled(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_ui_ns == null || node.Name != input.boa_ui_ns + ":" + "BTextEditorLabeled")
            {
                return;
            }

            var Document = input.Document;

            var newElement = Document.CreateElement("BInput");

            TransferAttribute(node, "Label", newElement, "hintText");
            TransferAttribute(node, "Text", newElement, "value");

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        public static void ParameterComponent(TransformerInput input)
        {
            var node = input.XmlNode;

            if (input.boa_BusinessComponents_ns == null || node.Name != input.boa_BusinessComponents_ns + ":" + "ParameterComponent")
            {
                return;
            }

            var Document = input.Document;

            var newElement = Document.CreateElement("BParameterComponent");

            TransferAttribute(node, "Label", newElement, "hintText");
            TransferAttribute(node, "Label", newElement, "labelText");
            TransferAttribute(node, "ParamType", newElement, "paramType");
            TransferAttribute(node, "IsAllOptionIncluded", newElement, "isAllOptionIncluded");
            TransferAttribute(node, "ParamValuesVisible", newElement, "paramValuesVisible");
            TransferAttribute(node, "SelectedParamCode", newElement, "selectedParamCode");
            TransferAttribute(node, "ParamCodeVisible", newElement, "paramCodeVisible");

            TransferNameAttribute(node, newElement, input.FieldDefinitions);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        public static void StackPanel(TransformerInput input)
        {
            var XmlNode = input.XmlNode;

            if (XmlNode.LocalName != "StackPanel")
            {
                return;
            }

            var Document = XmlNode.OwnerDocument;

            Debug.Assert(Document != null, nameof(Document) + " != null");

            var bGridSection = Document.CreateElement("BGridSection");

            var nodes = XmlNode.ChildNodes.ToList();

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

            if (XmlNode.ParentNode == null)
            {
                throw new ArgumentException();
            }

            XmlNode.ParentNode.InsertBefore(bGridSection, XmlNode);

            XmlNode.ParentNode.RemoveChild(XmlNode);
        }
        #endregion

        #region Methods
       

        

        static void TransferAttribute(XmlNode xamlNode, string xamlPropertyName, XmlElement newElement, string newAttribute)
        {
            if (xamlNode.Attributes?[xamlPropertyName] != null)
            {
                var attributeValue = xamlNode.Attributes?[xamlPropertyName]?.Value;

                newElement.SetAttribute(newAttribute, attributeValue);
            }
        }

        static void TransferNameAttribute(XmlNode xamlNode, XmlElement newElement, Dictionary<string, string> FieldDefinitions)
        {
            var xamlPropertyName = "x:Name";
            var newAttribute     = "x.Name";

            if (xamlNode.Attributes?[xamlPropertyName] == null)
            {
                return;
            }

            var fieldName = xamlNode.Attributes?[xamlPropertyName]?.Value;

            FieldDefinitions[fieldName] = XamlHelper.GetClassFullName(xamlNode);

            newElement.SetAttribute(newAttribute, fieldName);
        }
        #endregion
    }
}