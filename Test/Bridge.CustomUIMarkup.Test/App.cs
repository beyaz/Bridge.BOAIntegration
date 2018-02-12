using System.Text.Tokenizers;
using System.Windows;
using Bridge.Html5;
using Bridge.QUnit;

namespace Bridge.BOAIntegration
{
    public class App
    {
        #region Public Methods
        [Ready]
        public static void RunAll()
        {
            AllTest.Register();
        }
        #endregion

        #region Methods
        #endregion
    }
}