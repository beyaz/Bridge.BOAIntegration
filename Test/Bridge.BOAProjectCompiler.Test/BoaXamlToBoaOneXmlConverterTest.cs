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

            converter.TransformNodes();
        }

        [TestMethod]
        public void TransformNode()
        {
            var converter = new BoaXamlToBoaOneXmlConverter
            {
                InputXamlString = "<div xmlns:BOABusiness = 't'> <StackPanel Width='40'> <BOABusiness:AccountComponent type='text'/> </StackPanel>  </div>"
            };

            converter.TransformNodes();

            var expected = @"<div xmlns:BOABusiness=""t"">
  <BGridSection>
    <BGridRow>
      <BOABusiness:AccountComponent
        type=""text"" />
    </BGridRow>
  </BGridSection>
</div>";

            Assert.AreEqual(expected, converter.OutputXmlString);
        }
        #endregion
    }
}