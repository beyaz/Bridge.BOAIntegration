using System.Collections.Generic;

namespace Bridge.BOAIntegration
{
    static class MapHelper
    {
        #region Static Fields
        static readonly Dictionary<string, string[]> BooleanAttributes = new Dictionary<string, string[]>
        {
            {
                ComponentName.BAccountComponent.ToString(), new[]
                {
                    ComponentPropName.isVisibleBalance.ToString(),
                    ComponentPropName.isVisibleIBAN.ToString(),

                    ComponentPropName.showTaxNumberAndMernisVerifiedDialogMessage.ToString(),
                    ComponentPropName.showMernisServiceHealtyDialogMessage.ToString(),
                    ComponentPropName.showDialogMessages.ToString(),
                    ComponentPropName.showCustomerRecordingBranchWarning.ToString(),
                    ComponentPropName.showCustomerBranchAccountMessage.ToString(),
                    ComponentPropName.showBlackListDialogMessages.ToString(),
                    ComponentPropName.allowSharedAccountControl.ToString(),
                    ComponentPropName.allowDoubleSignatureControl.ToString(),
                    ComponentPropName.allow18AgeControl.ToString()
                }
            },
            {
                ComponentName.BComboBox.ToString(), new[]
                {
                    ComponentPropName.multiSelect.ToString(),
                    ComponentPropName.multiColumn.ToString(),
                    ComponentPropName.isAllOptionIncluded.ToString()
                }
            },
            {
                ComponentName.BInput.ToString(), new[]
                {
                    ComponentPropName.noWrap.ToString(),
                    ComponentPropName.multiLine.ToString()
                }
            },
            {
                ComponentName.BParameterComponent.ToString(), new[]
                {
                    ComponentPropName.disabled.ToString(),
                    ComponentPropName.paramValuesVisible.ToString(),
                    ComponentPropName.paramCodeVisible.ToString(),
                    ComponentPropName.isAllOptionIncluded.ToString()
                }
            }
        };

        static readonly Dictionary<string, string[]> NumberAttributes = new Dictionary<string, string[]>
        {
            {
                ComponentName.BDateTimePicker.ToString(), new[]
                    {ComponentPropName.size.ToString()}
            },
            {
                ComponentName.BGridRow.ToString(), new[]
                    {ComponentPropName.columnCount.ToString()}
            },
            {
                ComponentName.BCheckBox.ToString(), new[]
                    {ComponentPropName.size.ToString()}
            },
            {
                ComponentName.BComboBox.ToString(), new[]
                    {ComponentPropName.size.ToString()}
            },
            {
                ComponentName.BInput.ToString(), new[]
                {
                    ComponentPropName.rows.ToString(),
                    ComponentPropName.rowsMax.ToString(),
                    ComponentPropName.size.ToString()
                }
            },
            {
                ComponentName.BInputMask.ToString(), new[]
                    {ComponentPropName.size.ToString()}
            },
            {
                ComponentName.BParameterComponent.ToString(), new[]
                    {ComponentPropName.size.ToString()}
            }
        };
        #endregion

        #region Public Methods
        public static string[] GetBooleanAttributes(string componentName)
        {
            string[] booleanAttributes = null;

            if (BooleanAttributes.TryGetValue(componentName, out booleanAttributes) == false)
            {
                return null;
            }

            return booleanAttributes;
        }

        public static string[] GetNumberAttributes(string componentName)
        {
            string[] attributes = null;

            if (NumberAttributes.TryGetValue(componentName, out attributes) == false)
            {
                return null;
            }

            return attributes;
        }
        #endregion
    }
}