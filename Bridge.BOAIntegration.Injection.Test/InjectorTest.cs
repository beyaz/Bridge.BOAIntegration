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

                JSCodeWillbeInject = "X"
            };

            injector.InjectInitializerPart(injectInfo);

            Assert.AreEqual(@"var b_component_1 = __webpack_require__(5);
var b_localization_1 = __webpack_require__(21);
X
A
B
C", injectInfo.JSDataInjectedVersion);
        }


        [TestMethod]
        public void SetFirstStatementOfFunction()
        {
            var injector = new Injector();

            var injectInfo = new InjectInfo
            {
                JSData = @"A
B
value: function getDefaultPageRequest() {
C",

                JSCodeWillbeInject = "X"
            };

            injector.SetFirstStatementOfFunction(injectInfo, "getDefaultPageRequest");

            var expected = @"A
B
value: function getDefaultPageRequest() {X
C";
            Assert.AreEqual(expected, injectInfo.JSDataInjectedVersion);
        }

        #endregion

    }
}