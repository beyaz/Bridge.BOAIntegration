using System;
using System.Windows;
using System.Windows.Data;
using BOA.Types.Kernel.Account;

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

            if (attributeName == AttributeName.value && nodeName == "BDateTimePicker")
            {
                elementProps["onChange"] = Script.Write<object>(@"function(p0,value)
                {
                            me.BDateTimePicker_onChange_Handler(value,bindingPath);
                }");

                return true;
            }

            if (attributeName == AttributeName.value &&
                (nodeName == "BInputMask" || nodeName == "BInput"))
            {
                elementProps["onChange"] = Script.Write<object>(@"function(p0,value)
                {
                            me.BInputMask_onChange_Handler(value,bindingPath);
                }");

                return true;
            }

            if (attributeName == AttributeName.value && nodeName == "BInputNumeric")
            {
                elementProps["onChange"] = Script.Write<object>(@"function(p0,value)
                {
                            me.BInputNumeric_onChange_Handler(value,bindingPath);
                }");

                return true;
            }

            if (attributeName == "accountNumber" && nodeName == "BAccountComponent")
            {
                elementProps["onAccountSelect"] = Script.Write<object>(@"function(contract)
                {
                    me.BAccountComponent_onAccountSelect_Handler(contract,bindingPath,'accountNumber');
                }");

                return true;
            }

            if (attributeName == "selectedItems" && nodeName == "BComboBox")
            {
                elementProps["onSelect"] = Script.Write<object>(@"function(index,items)
                {
                            me.BComboBox_onSelect_Handler(index,items,bindingPath);
                }");

                return true;
            }

            return false;
        }
        #endregion

        #region Methods
        void BAccountComponent_onAccountSelect_Handler(AccountComponentAccountsContract selectedAccount, string bindingPath, string propName)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            if (propName == "accountNumber")
            {
                propertyPath.SetPropertyValue(selectedAccount.AccountNumber);
                return;
            }

            if (propName == "accountSuffix")
            {
                propertyPath.SetPropertyValue(selectedAccount.AccountSuffix);
                return;
            }

            // TODO acaba gelen contractın tam olarak bilgisi ne ? 
            if (propName == "selectedAccount")
            {
                propertyPath.SetPropertyValue(selectedAccount);
                return;
            }

            throw new ArgumentException(propName);
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
        #endregion
    }
}