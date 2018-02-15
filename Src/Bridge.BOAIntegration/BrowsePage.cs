using System;
using System.Diagnostics.CodeAnalysis;
using BOA.Common.Types;

namespace Bridge.BOAIntegration
{
    public class BrowsePage : BasePage
    {


        protected ReactElement BuildUI(string xmlUI,object prop)
        {
            var pageParams = Script.Write<object>("this.state.pageParams");
            var context    = Script.Write<object>("this.state.context");
            var me         = Script.Write<object>("this");
         

            var reactUiBuilder = new ReactUIBuilder
            {
                ComponentClassFinder = NodeModules.FindComponent,
                OnPropsEvaluated = (componentClass, componentProp) =>
                {

                    componentProp["pageParams"] = pageParams;
                    componentProp["context"]    = context;
                    componentProp["snapshot"]   = prop["snapshot"];

                    if (componentProp["snapshot"]["state"] == Script.Undefined)
                    {
                        // TODO:  combo da böle bişey oluyo ? 
                        componentProp["snapshot"]["state"] = ObjectLiteral.Create<object>();
                    }
                    var snapKey = componentProp["key"].As<string>();

                    componentProp["snapKey"] = snapKey;

                    var previousSnap = prop["dynamicProps"][snapKey];

                    componentProp = JsLocation._extend.Apply(null, componentProp, previousSnap);

                    return componentProp;
                }
            };


            return reactUiBuilder.Build(xmlUI, prop);
        }


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