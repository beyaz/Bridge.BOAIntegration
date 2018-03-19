using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.BOAProjectCompiler
{
    [TestClass]
    public class BoaXamlToBoaOneXmlConverterTest
    {
        [TestMethod]
        public void TransformNode()
        {
            var converter = new BoaXamlToBoaOneXmlConverter
            {
                InputXamlString = "<div xmlns:BOABusiness = 't'> <StackPanel Width='40'> <BOABusiness:AccountComponent type='text'/> </StackPanel>  </div>"
            };

            converter.TransformNodes();


            var expected = "<div xmlns:BOABusiness=\"t\"><BGridSection><BGridRow><BOABusiness:AccountComponent type=\"text\" /></BGridRow></BGridSection></div>";

           Assert.AreEqual(expected, converter.OutputXmlString);
        }
    }
}
