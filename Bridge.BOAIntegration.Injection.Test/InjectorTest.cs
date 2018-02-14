using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bridge.BOAIntegration.Injection.Test
{
    [TestClass]
    public class InjectorTest
    {
        #region Public Methods
        [TestMethod]
        public void InjectInitializerPart()
        {
            var injector = new Injector();

            var injectInfo = new InjectInfo
            {
                JSData = @"var b_component_1 = __webpack_require__(5);
var b_localization_1 = __webpack_require__(21);
A
B
C",

                InitializerJSCode = "X"
            };

            injector.InjectInitializerPart(injectInfo);

            Assert.AreEqual(@"var b_component_1 = __webpack_require__(5);
var b_localization_1 = __webpack_require__(21);
X
A
B
C", injectInfo.JSDataInjectedVersion);
        }
        #endregion


        [TestMethod]
        public void InjectInheritancePart()
        {
            var injector = new Injector();

            var injectInfo = new InjectInfo
            {
                JSData = @"A
}(b_framework_1.BrowsePage);
C",

                ViewTypeFullName = "BOA.One.Office.CardGeneral.DebitCard.CardTransactionList.View"
            };

            injector.InjectInheritancePart(injectInfo);

            Assert.AreEqual(@"A
}(BOA.One.Office.CardGeneral.DebitCard.CardTransactionList.View);
C", injectInfo.JSDataInjectedVersion);
        }

    }
}