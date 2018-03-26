using System.IO;
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
}