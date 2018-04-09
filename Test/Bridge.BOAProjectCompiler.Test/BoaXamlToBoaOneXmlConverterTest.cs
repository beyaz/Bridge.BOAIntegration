using System.IO;
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
            var xml = "<div width4 ='{Width3}'>" +
                      "   <div>{name}</div>" +
                      "   <div x='{Inner.Name3}'>{Width3}</div>" +
                      "<Component_1 Name5='{Inner.Name3}' />" +
                      "</div>";


            var generator = new UIBuilderCodeGenerator
            {
                RootNode    = XmlHelper.GetRootNode( XmlHelper.ClearXml(xml)),
                Caller      = "this",
                DataContext = "this",
                Output      = new PaddedStringBuilder()
            };

            generator.Generate();

            var result = generator.Output.ToString();
        }
        #endregion
    }
}