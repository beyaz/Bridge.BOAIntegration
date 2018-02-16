using System;
using System.Diagnostics.CodeAnalysis;
using BOA.Common.Types;
using BOA.Messaging;

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

        #region Public Properties
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public BState State { [Template("state")] get; }
        #endregion

        #region Public Methods
        public virtual string GetMessage(string groupName, string propertyName)
        {
            return Helper.GetMessage(groupName, propertyName);
        }

        public void SetState<T>(T state) where T : BState
        {
            setState(state);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void ShowError(string message, Result[] results)
        {
            var dialogHelper = NodeModules.BDialogHelper();

            Script.Write("dialogHelper.showError(this.state.context,message,results); ");
        }
        #endregion

        #region Methods
        protected ReactElement BuildUI(string xmlUI, object prop)
        {
            var reactUiBuilder = new ReactUIBuilder
            {
                ComponentClassFinder = NodeModules.FindComponent,
                OnPropsEvaluated     = OnPropsEvaluated
            };

            return reactUiBuilder.Build(new ReactUIBuilderInput
            {
                XmlUI  = xmlUI,
                Prop   = prop,
                Caller = this
            });
        }

        object OnPropsEvaluated(ReactUIBuilderData data)
        {
            var componentProp = data.CurrentComponentProp;

            var pageParams = State.PageParams;
            var context    = State.Context;

            var snapKey = componentProp[AttributeName.key].As<string>();

            componentProp[AttributeName.snapKey]    = snapKey;
            componentProp[AttributeName.pageParams] = pageParams;
            componentProp[AttributeName.context]    = context;
            componentProp[AttributeName.snapshot]   = State[AttributeName.snapshot][snapKey];
            var previousSnap = State[AttributeName.dynamicProps][snapKey];

            componentProp = JsLocation._extend.Apply(null, componentProp, previousSnap);

            // ReSharper disable once UnusedVariable
            var me = this;

            componentProp[AttributeName.Ref] = Script.Write<object>("function(r){  me.snaps[ snapKey ] = r;  }");

            return componentProp;
        }

        extern void setState(object state);
        #endregion
    }
}