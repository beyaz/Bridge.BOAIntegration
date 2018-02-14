using System;
using System.Diagnostics.CodeAnalysis;
using BOA.Common.Types;

namespace Bridge.BOAIntegration
{
    public class BrowsePage : BasePage
    {
        #region Constructors
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public BrowsePage(object props)
        {
            // call base constructor
            Script.Write(@"Bridge.$BOAIntegration.BrowsePageInTypeScript.prototype.constructor.apply(this,[props]);");
        }
        #endregion

        #region Public Methods
        public string GetMessage(string groupName, string propertyName)
        {
            return NodeModules.getMessage().Call(null, groupName, propertyName).As<string>();
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void ShowError(string message, Result[] results)
        {
            var dialogHelper = NodeModules.BDialogHelper();

            Script.Write(" dialogHelper.showError(this.state.context,message,results); ");
        }
        #endregion

        #region Methods
        [Name("setState")]
        protected extern void SetState(object state);
        #endregion
    }
}