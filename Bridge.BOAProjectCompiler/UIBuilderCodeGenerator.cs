using System;
using System.Diagnostics;
using System.Linq;
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
            Output.AppendLine("RenderCount                  = " + Caller + ".RenderCount" + ",");
            Output.AppendLine("TypeScriptWrittenJsObject    = " + Caller + ".TypeScriptVersion");

            Output.PaddingCount--;
            Output.AppendLine("};");

            Output.AppendLine("");

            RootNode.ForOwnAndChildNodes(NormalizeInnerHTML);

            WriteNode(RootNode);
        }
        #endregion

        #region Methods
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

        void Write(BindingInfoContract contract)
        {
            Output.Append("new System.Windows.Data.BindingInfoContract" + Environment.NewLine);
            Output.AppendLine("{");
            Output.PaddingCount++;

            Output.AppendLine($"BindingMode = System.Windows.Data.BindingMode.{contract.BindingMode.ToString()},");
            Output.AppendLine($"SourcePath  = \"{contract.SourcePath}\"");

            Output.PaddingCount--;
            Output.AppendWithPadding("}");
        }

        static bool TryToHandleAsMessagingAccess(AttributeData data)
        {
            var isMessagingExpression = MessagingResolver.IsMessagingExpression(data.attributeValue);
            if (isMessagingExpression)
            {
                var pair = MessagingResolver.GetMessagingExpressionValue(data.attributeValue);

                data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = BOA.Messaging.MessagingHelper.GetMessage(\"{pair.Key}\",\"{pair.Value}\");");
                return true;
            }

            return false;
        }
        bool TryToHandleAsBindingExpression(AttributeData data)
        {
            var bindingInfoContract = BindingExpressionParser.TryParse(data.attributeValue);
            if (bindingInfoContract != null)
            {
                data.Output.AppendWithPadding($"attributes[\"{data.attributeName}\"] = ");
                Write(bindingInfoContract);
                data.Output.Append(";");
                data.Output.Append(Environment.NewLine);
                return  true;
            }

            return false;
        }

        static bool TryToHandleAsEvent(AttributeData data)
        {
            if (data.attributeName == "onClick")
            {
                data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = {data.Caller + "[\"" + data.attributeValue + "\"]"};");

                return true;
            }

            return false;
        }

        static bool TryToHandleAsBoolean(AttributeData data)
        {
            var booleanAttributes = MapHelper.GetBooleanAttributes(data.componentName);

            var isBoolenAttribute = booleanAttributes?.Contains(data.attributeName) == true;
            if (isBoolenAttribute)
            {
                if (data.attributeValue.ToUpperEN() == "FALSE")
                {
                    data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = false.As<object>();");
                    return true;
                }

                if (data.attributeValue.ToUpperEN() == "TRUE")
                {
                    data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = true.As<object>();");
                    return true; 
                }

                throw new ArgumentException($"{data.componentName} -> {data.attributeName} must be boolan (false/true)");
            }

            return false;
        }

        static bool TryToHandleAsNumber(AttributeData data)
        {

            var numberAttributes = MapHelper.GetNumberAttributes(data.componentName);
            var isNumberProperty = numberAttributes?.Contains(data.attributeName) == true;
            if (isNumberProperty)
            {
                data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = {data.attributeValue};");
                return true;
            }

            return false;
        }

        static bool TryToHandleAsString(AttributeData data)
        {

            data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = \"{data.attributeValue}\";");

            return true;
        }

        class AttributeData
        {
            public string attributeName { get;  set; }
            public string attributeValue { get;  set; }
            public string componentName { get;  set; }
            public PaddedStringBuilder Output { get; set; }
            public string Caller { get; set; }

        }

        void WriteNode(XmlNode node)
        {
            var skipProcessChildNodes = false;

            var componentName = node.Name;

            var pipe = new Func<AttributeData, bool>[]
            {
                TryToHandleAsMessagingAccess,
                TryToHandleAsBindingExpression,
                TryToHandleAsEvent,
                TryToHandleAsBoolean,
                TryToHandleAsNumber,
                TryToHandleAsString
            };

            if (node.Attributes != null)
            {
                Output.AppendLine("attributes = Bridge.ObjectLiteral.Create<object>();");
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    
                    foreach (var handler in pipe)
                    {
                        var isHandled = handler(new AttributeData
                        {
                            attributeValue = attribute.Value,
                            attributeName = attribute.Name,
                            componentName = componentName,
                            Output = Output,
                            Caller = Caller
                        });
                        if (isHandled)
                        {
                            break;
                        }
                    }
                }

                if (componentName == "BComboBox")
                {
                    var columns = ((XmlElement) node).GetElementsByTagName("ComboBoxColumn").ToList();

                    if (columns.Count > 0)
                    {
                        skipProcessChildNodes = true;

                        Output.AppendLine("attributes[\"columns\"] = new object[0];");

                        Output.AppendLine("temp = Bridge.ObjectLiteral.Create<object>();");
                        foreach (var columnNode in columns)
                        {
                            Output.AppendLine("temp[\"key\"] = \"" + columnNode?.Attributes?["key"].Value + "\";");

                            var columnName          = columnNode?.Attributes?["Name"].Value;
                            var bindingInfoContract = BindingExpressionParser.TryParse(columnName);
                            if (bindingInfoContract != null)
                            {
                                Output.AppendLine("temp[\"name\"] = " + bindingInfoContract.SourcePath.Replace(".", "?.") + ";");
                                Output.AppendLine("attributes[\"columns\"].As<object[]>().Push(temp);");

                                continue;
                            }

                            Output.AppendLine("temp[\"name\"] = \"" + columnNode?.Attributes?["Name"] + "\";");
                            Output.AppendLine("attributes[\"columns\"].As<object[]>().Push(temp);");
                        }
                    }
                }
            }
            else
            {
                Output.AppendLine("attributes = null;");
            }

            Output.AppendLine($"builder.Create(\"{componentName}\" , attributes);");

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
        #endregion
    }
}