using System;
using Bridge.Html5;

namespace Bridge.BOAIntegration
{
    public class App
    {
        #region Public Methods
        [Ready]
        public static void RunAll()
        {
            ReactUIBuilderTest.Register();


            ReactUIBuilderBOAVersionTest.Register();
        }
        #endregion
    }
}