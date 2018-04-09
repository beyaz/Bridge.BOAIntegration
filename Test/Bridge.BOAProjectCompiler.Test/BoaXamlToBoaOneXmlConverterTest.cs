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
}