using System;
using System.Diagnostics.CodeAnalysis;
using BOA.Common.Types;
using BOA.Messaging;
using BOA.UI.Types;
using Bridge.jQuery2;

namespace Bridge.BOAIntegration
{

    public class BrowsePage : BasePage
    {
        public event EventHandler LoadCompleted;

        public bool CanExecuteAction(string commandName)
        {
            return true;
        }

      

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public void ShowStatusMessage(string message, DialogTypes dialogType)
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");
        }

        public void ClearStatusMessage()
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.clearStatusMessage(); ");
        }
        

        #region Public Properties
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public BState State { [Template("$TypeScriptVersion.state")] get; }


        public object Data => State.PageParams.Data;
        #endregion

        #region Public Methods
        public virtual string GetMessage(string groupName, string propertyName)
        {
            return MessagingHelper.GetMessage(groupName, propertyName);
        }

        public void SetState<T>(T state) where T : BState
        {
            setState(state);
        }


        protected Array DataSource
        {
            set
            {
                var newState = ObjectLiteral.Create<object>();

                newState["dataSource"] = value;

                setState(newState);
            }
        }



        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void ShowError(string message, Result[] results)
        {
            var dialogHelper = NodeModules.BDialogHelper();

            Script.Write("dialogHelper.showError(this.$TypeScriptVersion.state.context,message,results); ");
        }
        #endregion

        #region Methods

        protected string XmlUI { get; set; }

        public int RenderCount { get; private set; }
        public ReactElement render()
        {
            

            if (RenderCount == 0)
            {
                jQuery.Ready(() => { LoadCompleted?.Invoke(this, null); });
            }

            RenderCount++;

            return BuildUI(XmlUI);
        }
        protected ReactElement BuildUI(string xmlUI)
        {
            var reactUiBuilder = new ReactUIBuilder
            {
                ComponentClassFinder = NodeModules.FindComponent,
                OnPropsEvaluated     = OnPropsEvaluated,
                OnBeforeStartToProcessAttribute = OnBeforeStartToProcessAttribute
            };

            return reactUiBuilder.Build(new ReactUIBuilderInput
            {
                XmlUI  = xmlUI,
                DataContext   = this,
                Caller = this
            });
        }

        void OnBeforeStartToProcessAttribute(ReactUIBuilderData data)
        {
            MakeLowercaseFirstChar(data);
        }

         static void MakeLowercaseFirstChar(ReactUIBuilderData data)
        {
            data.CurrentAttributeName = data.CurrentAttributeName[0].ToString().ToLower() + data.CurrentAttributeName.Substring(1);
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

            componentProp[AttributeName.Ref] = Script.Write<object>("function(r){  me.$TypeScriptVersion.snaps[ snapKey ] = r;  }");

            if (data.CurrentComponentName == "BInputMask")
            {
                // TODO: bug fix value null olduğunda _isCorrectFormatText metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.value] == null)
                {
                    componentProp[AttributeName.value] = "";
                }
            }


            if (data.CurrentComponentName == "BComboBox")
            {
                // TODO: bug fix value null olduğunda organizeState metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.dataSource] == null)
                {
                    componentProp[AttributeName.dataSource] = new object[0];
                }
            }



            return componentProp;
        }

        [Template("$TypeScriptVersion.setState({0})")]
        extern void setState(object state);

        [Template("$TypeScriptVersion.forceUpdate()")]
        protected extern void forceUpdate();



        protected void ForceRender()
        {
            forceUpdate();
        }
        #endregion
    }
}