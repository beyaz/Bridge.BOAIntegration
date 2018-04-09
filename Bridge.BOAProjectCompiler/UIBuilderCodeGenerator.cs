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
        #endregion
    }
}