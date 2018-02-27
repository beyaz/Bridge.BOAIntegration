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
        #region Public Events
        public event EventHandler LoadCompleted;
        #endregion

        #region Public Properties
        public object Data => State.PageParams.Data;

        public int RenderCount { get; private set; }

        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public BState State { [Template("$TypeScriptVersion.state")] get; }

        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public object TypeScriptVersion { [Template("$TypeScriptVersion")] get; }
        #endregion

        #region Properties
        protected Array DataSource
        {
            set
            {
                var newState = ObjectLiteral.Create<object>();

                newState["dataSource"] = value;

                setState(newState);
            }
        }

        protected string XmlUI { get; set; }
        #endregion

        #region Public Methods
        public bool CanExecuteAction(string commandName)
        {
            return true;
        }

        public void ClearStatusMessage()
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.clearStatusMessage(); ");
        }

        public virtual string GetMessage(string groupName, string propertyName)
        {
            return MessagingHelper.GetMessage(groupName, propertyName);
        }

        public ReactElement render()
        {
            if (RenderCount == 0)
            {
                jQuery.Ready(() => { LoadCompleted?.Invoke(this, null); });
            }

            RenderCount++;

            return BuildUI(XmlUI);
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

            Script.Write("dialogHelper.showError(this.$TypeScriptVersion.state.context,message,results); ");
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public void ShowStatusMessage(string message, DialogTypes dialogType)
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");
        }
        #endregion

        #region Methods
        protected ReactElement BuildUI(string xmlUI)
        {
            var reactUiBuilder = new ReactUIBuilderBOAVersion
            {
                XmlUI                     = xmlUI,
                DataContext               = this,
                Caller                    = this,
                TypeScriptWrittenJsObject = TypeScriptVersion
            };

            return reactUiBuilder.Build();
        }

        protected void ForceRender()
        {
            forceUpdate();
        }

        [Template("$TypeScriptVersion.forceUpdate()")]
        protected extern void forceUpdate();

        [Template("$TypeScriptVersion.setState({0})")]
        extern void setState(object state);
        #endregion
    }
}