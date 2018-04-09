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

            Output.AppendLine("var builder = new UIBuilder");
            Output.AppendLine("{");
            Output.PaddingCount++;

            Output.AppendLine("Caller      = " + Caller + ",");
            Output.AppendLine("DataContext = " + DataContext);

            Output.PaddingCount--;
            Output.AppendLine("};");

            Output.AppendLine("");

            WriteNode(RootNode);
        }
        #endregion

        #region Methods
        void WriteNode(XmlNode node)
        {
            var nodeName = node.Name;

            if (node.Attributes != null)
            {
                Output.AppendLine("attributes = ObjectLiteral.Create<object>();");
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    var attributeValue = attribute.Value;
                    var bindingInfoContract = BindingExpressionParser.TryParse(attributeValue);
                    if (bindingInfoContract != null)
                    {
                        Output.AppendWithPadding($"attributes[\"{attribute.Name}\"] = ");
                        Write(bindingInfoContract);
                        Output.Append(";");
                        continue;
                    }

                    // using System -> .As<object>()
                    Output.AppendLine($"attributes[\"{attribute.Name}\"] = {attributeValue};");
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
            
            Output.AppendLine("new BindingInfoContract");
            Output.AppendLine("{");
            Output.PaddingCount++;

            Output.AppendLine($"BindingMode = BindingMode.{contract.BindingMode.ToString()},");
            Output.AppendLine($"SourcePath  = {contract.SourcePath}");



            Output.PaddingCount--;
            Output.AppendWithPadding("}");
        }
        #endregion
    }
}