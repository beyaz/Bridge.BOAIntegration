using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using BOA.BOA.Common.Helpers;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    class BoaXamlToBoaOneXmlConverter
    {
        #region Fields
        readonly Dictionary<string, string> FieldDefinitions = new Dictionary<string, string>();
        IDictionary<string, string>         _namespaces;
        #endregion

        #region Public Properties
        public string InputXamlString { get; set; }

        public string OutputXmlString
        {
            get
            {
                if (RootNodeIsBrowseForm())
                {
                    return XmlHelper.PrettyXml(RootNode.FirstChild.OuterXml);
                }

                return XmlHelper.PrettyXml(RootNode.OuterXml);
            }
        }

        public XmlNode RootNode { get; set; }
        #endregion

        #region Properties
        string      boa_BusinessComponents_ns => Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI.BusinessComponents;assembly=BOA.UI.BusinessComponents").Key;
        string      boa_ui_ns                 => Namespaces.FirstOrDefault(x => x.Value == "clr-namespace:BOA.UI;assembly=BOA.UI").Key;
        XmlDocument Document                  => RootNode.OwnerDocument;

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

        #region Public Methods
        public string GenerateCsharpCode()
        {
            var sb = new PaddedStringBuilder();

            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using BOA.Common.Helpers;");
            sb.AppendLine("using Bridge.BOAIntegration;");

            var fullTypeName = GetClassName();
            var nsName       = fullTypeName.Substring(0, fullTypeName.LastIndexOf(".", StringComparison.Ordinal));
            var typeName     = fullTypeName.Substring(fullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1);

            sb.AppendLine("namespace " + nsName);
            sb.AppendLine("{");
            sb.PaddingLength = 4;

            sb.AppendLine($"public partial class {typeName} : BrowsePage");
            sb.AppendLine("{");
            sb.PaddingLength += 4;

            foreach (var fieldNameFieldTypePair in FieldDefinitions)
            {
                sb.AppendLine(fieldNameFieldTypePair.Value + " " + fieldNameFieldTypePair.Key+";");
            }

            sb.AppendLine("void InitializeComponent()");
            sb.AppendLine("{");
            sb.PaddingLength += 4;

            sb.AppendLine("XmlUI = @" + '"' + OutputXmlString.Replace("\"", "\"\"") + '"' + ";");

            sb.AppendLine("");
            sb.AppendLine("// EvaluateInWhichCaseRenderMethodWillBeCall");

            var controlGridDataSourceBindingPath = GetBrowseForm_ControlGridDataSource_BindingPath();
            if (controlGridDataSourceBindingPath.StartsWith("Model."))
            {
                sb.AppendLine("this.OnPropertyChanged(nameof(Model), ForceRender);");
                sb.AppendLine("this.OnPropertyChanged(nameof(Model), ()=>{ ControlGridDataSource = new string[0]; });");
                
                sb.AppendLine("this.OnPropertyChanged(nameof(Model), () =>");

                sb.AppendLine("{");
                sb.PaddingLength += 4;

                sb.AppendLine("Model?.OnPropertyChanged( \"" + controlGridDataSourceBindingPath.Substring("Model.".Length) + "\" , () =>");

                sb.AppendLine("{");
                sb.PaddingLength += 4;

                sb.AppendLine("ControlGridDataSource = " + controlGridDataSourceBindingPath + ".ToArray();");

                sb.PaddingLength -= 4;
                sb.AppendLine("});");

                sb.PaddingLength -= 4;
                sb.AppendLine("});");
            }
            else
            {
                if (controlGridDataSourceBindingPath.Contains("."))
                {
                    throw new ArgumentException("controlGridDataSourceBindingPath.Contains('.')");
                }

                sb.AppendLine("this.OnPropertyChanged(nameof("+ controlGridDataSourceBindingPath + "), () =>");
                sb.AppendLine("{");
                sb.PaddingLength += 4;

                sb.AppendLine("ControlGridDataSource = " + controlGridDataSourceBindingPath + "?.ToArray();");

                sb.PaddingLength -= 4;
                sb.AppendLine("});");


            }

            sb.PaddingLength -= 4;
            sb.AppendLine("}"); // end of method

            sb.PaddingLength -= 4;
            sb.AppendLine("}"); // end of class

            sb.PaddingLength -= 4;
            sb.AppendLine("}"); // end of namespace

            return sb.ToString();
        }

        public string GetBrowseForm_ControlGridDataSource_BindingPath()
        {
            var value = RootNode.Attributes?.GetNamedItem("ControlGridDataSource")?.Value;
            if (value == null)
            {
                return null;
            }

            return value.Replace("{", "").Replace("}", "").Replace("Binding ", "").Trim();
        }

        public string GetClassName()
        {
            var fullTypeName = RootNode.Attributes?.GetNamedItem("x:Class")?.Value;
            if (fullTypeName == null)
            {
                return null;
            }

            return fullTypeName;
        }

        public bool RootNodeIsBrowseForm()
        {
            return RootNode.Name == boa_ui_ns + ":BrowseForm";
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
                var attributeValue = xamlNode.Attributes?[xamlPropertyName]?.Value;

                newElement.SetAttribute(newAttribute, attributeValue);
            }
        }

        void ApplyComponentTransforms()
        {
            Transform_BMaskedEditorLabeled();
            Transform_BDateTimeEditorLabeled();
            Transform_AccountComponent();
            Transform_BComboEditorMultiSelect();
            Transform_ParameterComponent();
            Transform_BTextEditorLabeled();
            Transform_BBranchComponent();
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

        void TransferNameAttribute(XmlNode xamlNode, XmlElement newElement)
        {
            var xamlPropertyName = "x:Name";
            var newAttribute = "x.Name";

            if (xamlNode.Attributes?[xamlPropertyName] == null)
            {
                return;
            }

            var fieldName = xamlNode.Attributes?[xamlPropertyName]?.Value;

            FieldDefinitions[fieldName] = GetNamespaceName(xamlNode) + "." + xamlNode.LocalName;

            newElement.SetAttribute(newAttribute, fieldName);
        }

         static string GetNamespaceName(XmlNode xamlNode)
        {
            return xamlNode.NamespaceURI.Split(';').FirstOrDefault(x => x != null && x.Contains("clr-namespace:"))?.RemoveFromStart("clr-namespace:");
        }

        void Transform_AccountComponent()
        {
            if (boa_BusinessComponents_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_BusinessComponents_ns + ":" + "AccountComponent").ToList().ForEach(Transform_AccountComponent);
        }

        void Transform_ParameterComponent()
        {
            if (boa_BusinessComponents_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_BusinessComponents_ns + ":" + "ParameterComponent").ToList().ForEach(Transform_ParameterComponent);
        }
        void Transform_BTextEditorLabeled()
        {
            if (boa_ui_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_ui_ns + ":" + "BTextEditorLabeled").ToList().ForEach(Transform_BTextEditorLabeled);
        }
        void Transform_BTextEditorLabeled(XmlNode node)
        {
            var newElement = Document.CreateElement("BInput");

            TransferAttribute(node, "Label", newElement, "hintText");
            TransferAttribute(node, "Text", newElement, "value");

            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }
        
        void Transform_ParameterComponent(XmlNode node)
        {
            var newElement = Document.CreateElement("BParameterComponent");

            TransferAttribute(node, "Label", newElement, "hintText");
            TransferAttribute(node, "Label", newElement, "labelText");
            TransferAttribute(node, "ParamType", newElement, "paramType");
            TransferAttribute(node, "IsAllOptionIncluded", newElement, "isAllOptionIncluded");
            TransferAttribute(node, "ParamValuesVisible", newElement, "paramValuesVisible");
            TransferAttribute(node, "SelectedParamCode", newElement, "selectedParamCode");
            TransferAttribute(node, "ParamCodeVisible", newElement, "paramCodeVisible");
            
            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
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

        void Transform_BBranchComponent(XmlNode node)
        {
            var newElement = Document.CreateElement("BBranchComponent");

            TransferAttribute(node, "SelectedBranchId", newElement, "selectedBranchId");

            newElement.SetAttribute("mode", "horizontal");



            TransferAttribute(node, "Label", newElement, "labelText");

            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        void Transform_BBranchComponent()
        {
            if (boa_BusinessComponents_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_BusinessComponents_ns + ":" + "BranchComponent").ToList().ForEach(Transform_BBranchComponent);
        }


        void Transform_BComboEditorMultiSelect()
        {
            if (boa_ui_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_ui_ns + ":" + "BComboEditorMultiSelect").ToList().ForEach(Transform_BComboEditorMultiSelect);
        }

        void Transform_BComboEditorMultiSelect(XmlNode node)
        {
            var newElement = Document.CreateElement("BComboBox");

            TransferAttribute(node, "Label", newElement, "labelText");
            TransferAttribute(node, "ItemsSource", newElement, "dataSource");
            TransferAttribute(node, "SelectedItems", newElement, "selectedItems");

            newElement.SetAttribute("displayLabelSeperator", ",");
            newElement.SetAttribute("multiSelect", "true");
            newElement.SetAttribute("multiColumn", "true");
            newElement.SetAttribute("isAllOptionIncluded", "true");

            var bfieldLayoutNode = ((XmlElement) node).GetElementsByTagName(boa_ui_ns + ":BFieldLayout").ToList().FirstOrDefault();

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

            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        void Transform_BDateTimeEditorLabeled(XmlNode node)
        {
            var newElement = Document.CreateElement("BDateTimePicker");

            newElement.SetAttribute("format", "DDMMYYYY");
            TransferAttribute(node, "Value", newElement, "value");
            TransferAttribute(node, "Label", newElement, "floatingLabelTextDate");
            TransferNameAttribute(node, newElement);

            node.ParentNode?.InsertBefore(newElement, node);
            node.ParentNode?.RemoveChild(node);
        }

        void Transform_BDateTimeEditorLabeled()
        {
            if (boa_ui_ns == null)
            {
                return;
            }

            Document.GetElementsByTagName(boa_ui_ns + ":" + "BDateTimeEditorLabeled").ToList().ForEach(Transform_BDateTimeEditorLabeled);
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