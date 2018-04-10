using System;
using System.Diagnostics;
using System.Windows.Data;
using System.Xml;
using BOA.Common.Helpers;

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
            var nodeName = node.Name;

            if (node.Attributes != null)
            {
                Output.AppendLine("attributes = Bridge.ObjectLiteral.Create<object>();");
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    var attributeValue = attribute.Value;
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

                    // using System -> .As<object>()
                    Output.AppendLine($"attributes[\"{attribute.Name}\"] = \"{attributeValue}\";");
                }
            }
            else
            {
                Output.AppendLine("attributes = null;");
            }

            Output.AppendLine($"builder.Create(\"{nodeName}\" , attributes);");

            if (!node.HasChildNodes)
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