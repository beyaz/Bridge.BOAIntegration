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
        #region Static Fields
        static readonly Func<AttributeData, bool>[] PropertyResolverPipe =
        {
            PropertyResolvers.TryToHandleAsMessagingAccess,
            PropertyResolvers.TryToHandleAsBindingExpression,
            PropertyResolvers.TryToHandleAsEvent,
            PropertyResolvers.TryToHandleAsBoolean,
            PropertyResolvers.TryToHandleAsNumber,
            PropertyResolvers.TryToHandleAsString
        };
        #endregion

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

            RootNode.ForOwnAndChildNodes(Helper.NormalizeInnerHTML);

            WriteNode(RootNode);
        }
        #endregion

        #region Methods
        static void AfterAttributesProcessed(AfterAttributesProcessedData data)
        {
            var output  = data.Output;
            var xmlNode = data.node;

            if (xmlNode.Name == "BComboBox")
            {
                var columns = ((XmlElement) xmlNode).GetElementsByTagName("ComboBoxColumn").ToList();

                if (columns.Count > 0)
                {
                    data.skipProcessChildNodes = true;

                    output.AppendLine("attributes[\"columns\"] = new object[0];");

                    output.AppendLine("temp = Bridge.ObjectLiteral.Create<object>();");
                    foreach (var columnNode in columns)
                    {
                        output.AppendLine("temp[\"key\"] = \"" + columnNode?.Attributes?["key"].Value + "\";");

                        var columnName          = columnNode?.Attributes?["Name"].Value;
                        var bindingInfoContract = BindingExpressionParser.TryParse(columnName);
                        if (bindingInfoContract != null)
                        {
                            output.AppendLine("temp[\"name\"] = " + bindingInfoContract.SourcePath.Replace(".", "?.") + ";");
                            output.AppendLine("attributes[\"columns\"].As<object[]>().Push(temp);");

                            continue;
                        }

                        output.AppendLine("temp[\"name\"] = \"" + columnNode?.Attributes?["Name"] + "\";");
                        output.AppendLine("attributes[\"columns\"].As<object[]>().Push(temp);");
                    }
                }
            }
        }

        void WriteNode(XmlNode node)
        {
            var componentName = node.Name;

            var afterAttributesProcessedData = new AfterAttributesProcessedData
            {
                node   = node,
                Output = Output
            };

            if (node.Attributes != null)
            {
                Output.AppendLine("attributes = Bridge.ObjectLiteral.Create<object>();");
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    foreach (var handler in PropertyResolverPipe)
                    {
                        var attributeData = new AttributeData
                        {
                            attributeValue = attribute.Value,
                            attributeName  = attribute.Name,
                            componentName  = componentName,
                            Output         = Output,
                            Caller         = Caller
                        };
                        var isHandled = handler(attributeData);
                        if (isHandled)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                Output.AppendLine("attributes = null;");
            }

            AfterAttributesProcessed(afterAttributesProcessedData);

            Output.AppendLine($"builder.Create(\"{componentName}\" , attributes);");

            if (!node.HasChildNodes || afterAttributesProcessedData.skipProcessChildNodes)
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

        static class PropertyResolvers
        {
            #region Public Methods
            public static bool TryToHandleAsBindingExpression(AttributeData data)
            {
                var bindingInfoContract = BindingExpressionParser.TryParse(data.attributeValue);
                if (bindingInfoContract != null)
                {
                    data.Output.AppendWithPadding($"attributes[\"{data.attributeName}\"] = ");
                    Helper.Write(data.Output, bindingInfoContract);
                    data.Output.Append(";");
                    data.Output.Append(Environment.NewLine);
                    return true;
                }

                return false;
            }

            public static bool TryToHandleAsBoolean(AttributeData data)
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

            public static bool TryToHandleAsEvent(AttributeData data)
            {
                if (data.attributeName == "onClick")
                {
                    data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = {data.Caller + "[\"" + data.attributeValue + "\"]"};");

                    return true;
                }

                return false;
            }

            public static bool TryToHandleAsMessagingAccess(AttributeData data)
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

            public static bool TryToHandleAsNumber(AttributeData data)
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

            public static bool TryToHandleAsString(AttributeData data)
            {
                data.Output.AppendLine($"attributes[\"{data.attributeName}\"] = \"{data.attributeValue}\";");

                return true;
            }
            #endregion
        }

        class AfterAttributesProcessedData
        {
            #region Public Properties
            public XmlNode             node                  { get; set; }
            public PaddedStringBuilder Output                { get; set; }
            public bool                skipProcessChildNodes { get; set; }
            #endregion
        }

        class AttributeData
        {
            #region Public Properties
            public string              attributeName  { get; set; }
            public string              attributeValue { get; set; }
            public string              Caller         { get; set; }
            public string              componentName  { get; set; }
            public PaddedStringBuilder Output         { get; set; }
            #endregion
        }

        class Helper
        {
            #region Methods
            internal static void NormalizeInnerHTML(XmlNode node)
            {
                if (node.NodeType != XmlNodeType.Text)
                {
                    return;
                }

                if (node.Attributes != null)
                {
                    return;
                }

                if (node.HasChildNodes)
                {
                    return;
                }

                var attribute = node.OwnerDocument?.CreateAttribute("innerHTML", string.Empty);

                Debug.Assert(attribute != null, nameof(attribute) + " != null");

                attribute.Value = node.Value;

                ((XmlElement) node.ParentNode)?.Attributes.Append(attribute);

                node.RemoveFromParent();
            }

            internal static void Write(PaddedStringBuilder output, BindingInfoContract contract)
            {
                output.Append("new System.Windows.Data.BindingInfoContract" + Environment.NewLine);
                output.AppendLine("{");
                output.PaddingCount++;

                output.AppendLine($"BindingMode = System.Windows.Data.BindingMode.{contract.BindingMode.ToString()},");
                output.AppendLine($"SourcePath  = \"{contract.SourcePath}\"");

                output.PaddingCount--;
                output.AppendWithPadding("}");
            }
            #endregion
        }
    }
}