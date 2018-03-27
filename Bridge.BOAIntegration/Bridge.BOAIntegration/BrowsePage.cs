using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BOA.Common.Types;
using BOA.Messaging;
using BOA.UI.Types;
using Bridge;
using Bridge.BOAIntegration;

namespace BOA.UI
{
    public class FormBase : WindowBase
    {
    }
}

namespace BOA.UI
{
    public class BDialogBox
    {
        public static DialogResponses Show(string message, DialogTypes dialogType, List<Result> resultList)
        {
            // TODO: fix error message
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");

            return DialogResponses.Ok;
        }

        public static DialogResponses Show(string message, DialogTypes dialogType, Result[] resultList)
        {
            // TODO: fix error message resultList :  readonly collection olmalı
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");

            return DialogResponses.Ok;
        }
    }
    public class BrowseForm : FormBase
    {
        public BDataGrid ControlGrid { get; set; }

        #region Public Properties
        public IEnumerable ControlGridDataSource
        {
            set
            {
                if (StateIsReadyToUpdate() == false)
                {
                    return;
                }

                var newState = ObjectLiteral.Create<object>();

                newState["dataSource"] = value;

                setState(newState);
            }
            get { return Script.Write<IEnumerable>("this.$TypeScriptVersion.state.dataSource"); }
        }
        #endregion

        #region Public Methods
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

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public void ShowStatusMessage(string message)
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");
        }

        public void ShowStatusMessage(string message, DialogTypes dialogType, List<Result> resultList)
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");
        }

        public void ShowStatusMessage(string message, DialogTypes dialogType, Result[] resultList)
        {
            // ReSharper disable once UnusedVariable
            var dialogHelper = NodeModules.BFormManager();

            Script.Write("dialogHelper.showStatusMessage(message); ");
        }
        #endregion

        #region Methods
        bool StateIsReadyToUpdate()
        {
            return TypeScriptVersion != null;
        }
        #endregion
    }
}