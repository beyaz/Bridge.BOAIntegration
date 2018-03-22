using System;
using System.Windows;
using System.Windows.Data;
using BOA.Common.Types;
using BOA.Types.Kernel.Account;
using Bridge.jQuery2;

namespace Bridge.BOAIntegration
{
    class TargetToSourceBinder
    {
        #region Fields
        internal string      attributeName;
        internal BindingInfo bindingInfo;
        internal object      DataContext;
        internal object      elementProps;
        internal string      nodeName;
        #endregion

        #region Public Methods
        public bool TryBind()
        {
            // ReSharper disable once UnusedVariable
            var bindingPath = bindingInfo.SourcePath.Path;

            // ReSharper disable once UnusedVariable
            var me = this;

            if (attributeName == AttributeName.value &&
                (nodeName == ComponentName.BDateTimePicker.ToString() ))
            {
                elementProps["onChange"] = (Action<jQueryEvent, DateTime?>) ((queryEvent, value) => { me.BDateTimePicker_onChange_Handler(value, bindingPath); });

                return true;
            }

            if (attributeName == AttributeName.value &&
                (nodeName == ComponentName.BInputMask.ToString() || nodeName == ComponentName.BInput.ToString()))
            {
                elementProps["onChange"] = (Action<jQueryEvent, string>) ((queryEvent, value) => { me.BInputMask_onChange_Handler(value, bindingPath); });

                return true;
            }
            if (attributeName == ComponentPropName.selectedBranchId.ToString() &&
                (nodeName == ComponentName.BBranchComponent.ToString()))
            {
                elementProps["onBranchSelect"] = (Action<BranchContract>)((selectedBranchContract) => { me.BBranchComponent_onBranchSelect_Handler(selectedBranchContract, bindingPath, ComponentPropName.selectedBranchId.ToString()); });

                return true;
            }

            

            if (attributeName == AttributeName.value && nodeName == ComponentName.BInputNumeric.ToString())
            {
                elementProps["onChange"] = (Action<jQueryEvent, string>) ((queryEvent, value) => { me.BInputNumeric_onChange_Handler(value, bindingPath); });

                return true;
            }

            if (nodeName == ComponentName.BAccountComponent.ToString() && attributeName == "accountNumber")
            {
                elementProps["onAccountSelect"] = (Action<AccountComponentAccountsContract>) (selectedAccount => { me.BAccountComponent_onAccountSelect_Handler(selectedAccount, bindingPath, "accountNumber"); });

                return true;
            }

            if (nodeName == ComponentName.BParameterComponent.ToString() && attributeName == "selectedParamCode")
            {
                elementProps["onParameterSelect"] = (Action<object>) (selectedParameterContract => { me.BParameterComponent_onParameterSelect_Handler(selectedParameterContract, bindingPath, "selectedParamCode"); });

                return true;
            }

            if (attributeName == "selectedItems" && nodeName == ComponentName.BComboBox.ToString())
            {
                elementProps["onSelect"] = (Action<int, object[]>) ((index, items) => { me.BComboBox_onSelect_Handler(index, items, bindingPath); });

                return true;
            }

            if (nodeName == ComponentName.BCheckBox.ToString() && attributeName == "checked")
            {
                elementProps["onCheck"] = (Action<jQueryEvent, bool>) ((e, isChecked) => { me.BCheckBox_onCheck_Handler(isChecked, bindingPath); });

                return true;
            }

            return false;
        }
        #endregion

        #region Methods


        void BBranchComponent_onBranchSelect_Handler(BranchContract selectedBranchContract, string bindingPath, string propName)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            if (propName == ComponentPropName.selectedBranchId.ToString())
            {
                selectedBranchContract = Utility.ConvertBOAJsonObjectToDotnetInstance<BranchContract>(selectedBranchContract);
                propertyPath.SetPropertyValue(selectedBranchContract.BranchId);
                return;
            }
            
            throw new ArgumentException(propName);
        }

        

        void BAccountComponent_onAccountSelect_Handler(AccountComponentAccountsContract selectedAccount, string bindingPath, string propName)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            if (propName == ComponentPropName.accountNumber.ToString()) 
            {
                propertyPath.SetPropertyValue(selectedAccount.AccountNumber);
                return;
            }

            if (propName == ComponentPropName.accountSuffix.ToString())
            {
                propertyPath.SetPropertyValue(selectedAccount.AccountSuffix);
                return;
            }

            // TODO acaba gelen contractın tam olarak bilgisi ne ? 
            if (propName == ComponentPropName.selectedAccount.ToString())
            {
                propertyPath.SetPropertyValue(selectedAccount);
                return;
            }

            throw new ArgumentException(propName);
        }

        void BCheckBox_onCheck_Handler(bool isChecked, string bindingPath)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            propertyPath.SetPropertyValue(isChecked);
        }

        // ReSharper disable once UnusedParameter.Local
        void BComboBox_onSelect_Handler(int index, object[] items, string bindingPath)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            propertyPath.SetPropertyValue(items);
        }

        void BDateTimePicker_onChange_Handler(DateTime? value, string bindingPath)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            propertyPath.SetPropertyValue(value.As<object>());
        }

        void BInputMask_onChange_Handler(string value, string bindingPath)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            propertyPath.SetPropertyValue(value);
        }

        void BInputNumeric_onChange_Handler(string value, string bindingPath)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            propertyPath.SetPropertyValue(value);
        }

        void BParameterComponent_onParameterSelect_Handler(dynamic parameterContract, string bindingPath, string propName)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            if (propName == "selectedParamCode")
            {
                propertyPath.SetPropertyValue(parameterContract.paramCode);
                return;
            }

            throw new ArgumentException(propName);
        }
        #endregion
    }
}