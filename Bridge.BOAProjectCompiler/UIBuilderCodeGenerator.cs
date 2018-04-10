using System;
using System.Diagnostics;
using System.Windows.Data;
using System.Xml;
using BOA.Common.Helpers;
using Bridge.BOAIntegration;

namespace Bridge.BOAProjectCompiler
{
    class UIBuilderCodeGenerator
    {
        #region Public Properties
        public string              Caller      { get; set; }
        public string              DataContext { get; set; }
        public PaddedStringBuilder Output      { get; set; }
        public XmlNode             RootNode    { get; set; }
        #endregion

        #region Public Methods
        public void Generate()
        {
            Output.AppendLine("object attributes = null;");
            Output.AppendLine("object temp       = null;");

            Output.AppendLine("var builder = new Bridge.BOAIntegration.UIBuilderForBOA");
            Output.AppendLine("{");
            Output.PaddingCount++;

            Output.AppendLine("Caller                       = " + Caller + ",");
            Output.AppendLine("DataContext                  = " + DataContext + ",");
            Output.AppendLine("RenderCount                  = " + Caller+ ".RenderCount" + ",");
            Output.AppendLine("TypeScriptWrittenJsObject    = " + Caller + ".TypeScriptVersion");
            
                

            Output.PaddingCount--;
            Output.AppendLine("};");

            Output.AppendLine("");

            RootNode.ForOwnAndChildNodes(NormalizeInnerHTML);

            WriteNode(RootNode);
        }
        #endregion

        static void NormalizeInnerHTML(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Text)
            {

                if (node.Attributes == null)
                {
                    if (node.HasChildNodes == false)
                    {
                        var attribute = node.OwnerDocument?.CreateAttribute("innerHTML", string.Empty);

                        Debug.Assert(attribute != null, nameof(attribute) + " != null");

                        attribute.Value = node.Value;

                        ((XmlElement) node.ParentNode)?.Attributes.Append(attribute);

                        node.RemoveFromParent();
                    }
                }
            }
        }

        #region Methods
        void WriteNode(XmlNode node)
        {

            var skipProcessChildNodes = false;

            var nodeName = node.Name;

            if (node.Attributes != null)
            {
                Output.AppendLine("attributes = Bridge.ObjectLiteral.Create<object>();");
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    var attributeValue = attribute.Value;

                    var isMessagingExpression = MessagingResolver.IsMessagingExpression(attributeValue);
                    if (isMessagingExpression)
                    {
                        var pair = MessagingResolver.GetMessagingExpressionValue(attributeValue);
                        
                        Output.AppendLine($"attributes[\"{attribute.Name}\"] = BOA.Messaging.MessagingHelper.GetMessage(\"{pair.Key}\",\"{pair.Value}\");");
                        continue;
                    }

                    var bindingInfoContract = BindingExpressionParser.TryParse(attributeValue);
                    if (bindingInfoContract != null)
                    {
                        Output.AppendWithPadding($"attributes[\"{attribute.Name}\"] = ");
                        Write(bindingInfoContract);
                        Output.Append(";");
                        Output.Append(Environment.NewLine);
                        continue;
                    }

                    if (attribute.Name == "onClick")
                    {
                        attributeValue = Caller + "." + attributeValue;
                    }

                    Output.AppendLine($"attributes[\"{attribute.Name}\"] = \"{attributeValue}\";");
                }

                if (nodeName == "BComboBox")
                {
                    var columns = ((XmlElement)node).GetElementsByTagName("ComboBoxColumn").ToList();

                    if (columns.Count > 0)
                    {
                        skipProcessChildNodes = true;

                        Output.AppendLine("attributes[\"columns\"] = new object[0];");


                        Output.AppendLine("temp = Bridge.ObjectLiteral.Create<object>();");
                        for (var i = 0; i < columns.Count; i++)
                        {
                            var columnNode = columns[i];
                            Output.AppendLine("temp[\"key\"] = \"" + columnNode?.Attributes?["key"].Value + "\";");


                            var columnName          = columnNode?.Attributes?["Name"].Value;
                            var bindingInfoContract = BindingExpressionParser.TryParse(columnName);
                            if (bindingInfoContract != null)
                            {
                                Output.AppendLine("temp[\"name\"] = " + bindingInfoContract.SourcePath.Replace(".","?.") + ";");
                                Output.AppendLine("attributes[\"columns\"].As<object[]>().Push(temp);");

                                continue;
                            }

                            Output.AppendLine($"temp[\"name\"] = \"" + columnNode?.Attributes?["Name"] + "\";");
                            Output.AppendLine("attributes[\"columns\"].As<object[]>().Push(temp);");
                        }
                    }

                }

            }
            else
            {
                Output.AppendLine("attributes = null;");
            }

            Output.AppendLine($"builder.Create(\"{nodeName}\" , attributes);");

            if (!node.HasChildNodes || skipProcessChildNodes)
            {
                Output.AppendLine("builder.EndOf();");
                return;
            }

            
                
            
            Output.PaddingCount++;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                Output.AppendLine("");
                WriteNode(childNode);
                Output.AppendLine("");
            }

            Output.PaddingCount--;
            Output.AppendLine("builder.EndOf();");
        }

        void Write(BindingInfoContract contract)
        {
            
            Output.Append(" new System.Windows.Data.BindingInfoContract"+Environment.NewLine);
            Output.AppendLine("{");
            Output.PaddingCount++;

            Output.AppendLine($"BindingMode = System.Windows.Data.BindingMode.{contract.BindingMode.ToString()},");
            Output.AppendLine($"SourcePath  = \"{contract.SourcePath}\"");



            Output.PaddingCount--;
            Output.AppendWithPadding("}");
        }
        #endregion
    }
}