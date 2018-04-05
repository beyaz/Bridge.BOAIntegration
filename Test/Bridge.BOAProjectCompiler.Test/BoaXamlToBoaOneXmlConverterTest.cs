using System.IO;
using System.Xml;
using BOA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.BOAProjectCompiler
{
    [TestClass]
    public class BoaXamlToBoaOneXmlConverterTest
    {
        #region Public Methods
        [TestMethod]
        public void TransformBrowsePage()
        {
            var path = @"D:\work\BOA.BusinessModules\Dev\BOA.CardGeneral.DebitCard\UI\BOA.UI.CardGeneral.DebitCard.CardTransactionList\CardTransactionListScreen\View.xaml";
            var converter = new BoaXamlToBoaOneXmlConverter
            {
                InputXamlString = File.ReadAllText(path)
            };

            converter.GenerateCsharpCode();
        }
        #endregion
    }

    [TestClass]
    public class UIBuilderCodeGeneratorTest
    {
        #region Public Methods
        [TestMethod]
        public void Generate()
        {
            var xml =
                @"<a y='g'>
<b h='2' />
<b h='3' /> 
<b h='4'>
    <c y='a1' />
</b>
</a>";
            var generator = new UIBuilderCodeGenerator
            {
                RootNode    = XmlHelper.GetRootNode(xml),
                Caller      = "this",
                DataContext = "this",
                Output      = new PaddedStringBuilder()
            };

            generator.Generate();

            var result = generator.Output.ToString();
        }
        #endregion
    }

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