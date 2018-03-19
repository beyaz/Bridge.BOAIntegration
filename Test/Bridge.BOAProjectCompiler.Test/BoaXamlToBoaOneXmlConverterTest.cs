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
                InputXamlString = "<div> <StackPanel Width='40'> <BOABusiness:AccountComponent type='text'/> </StackPanel>  </div>"
            };

            converter.TransformNodes();

           Assert.AreEqual("<div> <StackPanel Width='40'> <BOABusiness:AccountComponent type='text'/> </StackPanel>  </div>", converter.OutputXmlString);
        }
    }
}
