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
        void AfterAttributesProcessed(AfterAttributesProcessedData data)
        {
            Helper.HandleComboBoxColumns(data);
        }

        void WriteNode(XmlNode node)
        {
            var componentName = node.Name;

            var afterAttributesProcessedData = new AfterAttributesProcessedData
            {
                XmlNode = node,
                Output  = Output
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
                            AttributeValue = attribute.Value,
                            AttributeName  = attribute.Name,
                            ComponentName  = componentName,
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

            if (!node.HasChildNodes || afterAttributesProcessedData.SkipProcessChildNodes)
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
                var bindingInfoContract = BindingExpressionParser.TryParse(data.AttributeValue);
                if (bindingInfoContract != null)
                {
                    data.Output.AppendWithPadding($"attributes[\"{data.AttributeName}\"] = ");
                    Helper.Write(data.Output, bindingInfoContract);
                    data.Output.Append(";");
                    data.Output.Append(Environment.NewLine);
                    return true;
                }

                return false;
            }

            public static bool TryToHandleAsBoolean(AttributeData data)
            {
                var booleanAttributes = MapHelper.GetBooleanAttributes(data.ComponentName);

                var isBoolenAttribute = booleanAttributes?.Contains(data.AttributeName) == true;
                if (isBoolenAttribute)
                {
                    if (data.AttributeValue.ToUpperEN() == "FALSE")
                    {
                        data.Output.AppendLine($"attributes[\"{data.AttributeName}\"] = false.As<object>();");
                        return true;
                    }

                    if (data.AttributeValue.ToUpperEN() == "TRUE")
                    {
                        data.Output.AppendLine($"attributes[\"{data.AttributeName}\"] = true.As<object>();");
                        return true;
                    }

                    throw new ArgumentException($"{data.ComponentName} -> {data.AttributeName} must be boolan (false/true)");
                }

                return false;
            }

            public static bool TryToHandleAsEvent(AttributeData data)
            {
                if (data.AttributeName == "onClick")
                {
                    data.Output.AppendLine($"attributes[\"{data.AttributeName}\"] = {data.Caller + "[\"" + data.AttributeValue + "\"]"};");

                    return true;
                }

                return false;
            }

            public static bool TryToHandleAsMessagingAccess(AttributeData data)
            {
                var isMessagingExpression = MessagingResolver.IsMessagingExpression(data.AttributeValue);
                if (isMessagingExpression)
                {
                    var pair = MessagingResolver.GetMessagingExpressionValue(data.AttributeValue);

                    data.Output.AppendLine($"attributes[\"{data.AttributeName}\"] = BOA.Messaging.MessagingHelper.GetMessage(\"{pair.Key}\",\"{pair.Value}\");");
                    return true;
                }

                return false;
            }

            public static bool TryToHandleAsNumber(AttributeData data)
            {
                var numberAttributes = MapHelper.GetNumberAttributes(data.ComponentName);
                var isNumberProperty = numberAttributes?.Contains(data.AttributeName) == true;
                if (isNumberProperty)
                {
                    data.Output.AppendLine($"attributes[\"{data.AttributeName}\"] = {data.AttributeValue};");
                    return true;
                }

                return false;
            }

            public static bool TryToHandleAsString(AttributeData data)
            {
                data.Output.AppendLine($"attributes[\"{data.AttributeName}\"] = \"{data.AttributeValue}\";");

                return true;
            }
            #endregion
        }

        class AfterAttributesProcessedData
        {
            #region Public Properties
            public PaddedStringBuilder Output                { get; set; }
            public bool                SkipProcessChildNodes { get; set; }
            public XmlNode             XmlNode               { get; set; }
            #endregion
        }

        class AttributeData
        {
            #region Public Properties
            public string              AttributeName  { get; set; }
            public string              AttributeValue { get; set; }
            public string              Caller         { get; set; }
            public string              ComponentName  { get; set; }
            public PaddedStringBuilder Output         { get; set; }
            #endregion
        }

        class Helper
        {
            #region Methods
            internal static void HandleComboBoxColumns(AfterAttributesProcessedData data)
            {
                var output  = data.Output;
                var xmlNode = data.XmlNode;

                if (xmlNode.Name == "BComboBox")
                {
                    var columns = ((XmlElement) xmlNode).GetElementsByTagName("ComboBoxColumn").ToList();

                    if (columns.Count > 0)
                    {
                        data.SkipProcessChildNodes = true;

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