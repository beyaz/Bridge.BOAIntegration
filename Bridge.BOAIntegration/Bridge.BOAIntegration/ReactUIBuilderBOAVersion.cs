using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Bridge.BOAIntegration
{
    class ReactUIBuilderBOAVersion : ReactUIBuilder
    {
        #region Constants
        const string FALSE = "FALSE";

        const string TRUE = "TRUE";
        #endregion

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
                    {ComponentPropName.disabled.ToString()}
            }
        };

        static readonly Dictionary<string, string[]> NumberAttributes = new Dictionary<string, string[]>
        {
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

        #region Fields
        internal object TypeScriptWrittenJsObject;
        #endregion

        #region Constructors
        public ReactUIBuilderBOAVersion()
        {
            ComponentClassFinder = NodeModules.FindComponent;
        }
        #endregion

        #region Properties
        BState State => TypeScriptWrittenJsObject[AttributeName.state].As<BState>();
        #endregion

        #region Methods
        internal static void EvaluateBooleanValues(string componentName, object componentProp)
        {
            string[] booleanAttributes = null;

            if (BooleanAttributes.TryGetValue(componentName, out booleanAttributes) == false)
            {
                return;
            }

            var length = booleanAttributes.Length;
            for (var i = 0; i < length; i++)
            {
                var attributeName = booleanAttributes[i];
                var stringValue   = componentProp[attributeName] as string;
                if (stringValue == null)
                {
                    continue;
                }

                if (stringValue.ToUpper() == FALSE)
                {
                    componentProp[attributeName] = Script.Write<object>("false");
                    continue;
                }

                if (stringValue.ToUpper() == TRUE)
                {
                    componentProp[attributeName] = Script.Write<object>("true");
                    continue;
                }

                throw new ArgumentException($"{componentName} -> {attributeName} must be boolan (false/true)");
            }
        }

        internal static void EvaluateNumberValues(string componentName, object componentProp)
        {
            string[] attributes = null;

            if (NumberAttributes.TryGetValue(componentName, out attributes) == false)
            {
                return;
            }

            var length = attributes.Length;
            for (var i = 0; i < length; i++)
            {
                var attributeName = attributes[i];
                var stringValue   = componentProp[attributeName] as string;
                if (stringValue == null)
                {
                    continue;
                }

                // ReSharper disable once UnusedVariable
                var intValue = int.Parse(stringValue);

                componentProp[attributeName] = Script.Write<object>("intValue");
            }
        }

        protected override void BeforeStartToProcessAttribute(string attributeName, string attributeValue)
        {
            base.BeforeStartToProcessAttribute(attributeName, attributeValue);

            // MakeLowercaseFirstChar
            CurrentAttributeName = CurrentAttributeName[0].ToString().ToLower() + CurrentAttributeName.Substring(1);
        }

        protected override void OnPropsEvaluated(PropsEvaluatedEventArgs data)
        {
            var componentProp = data.CurrentComponentProp;

            var pageParams = State.PageParams;
            var context    = State.Context;

            var snapKey = componentProp[AttributeName.key].As<string>();
            if (snapKey == null)
            {
                throw new InvalidOperationException(nameof(snapKey) + " not found.");
            }

            componentProp[AttributeName.snapKey]    = snapKey;
            componentProp[AttributeName.pageParams] = pageParams;
            componentProp[AttributeName.context]    = context;
            componentProp[AttributeName.snapshot]   = State[AttributeName.snapshot][snapKey];
            var previousSnap = State[AttributeName.dynamicProps][snapKey];

            componentProp = JsLocation._extend.Apply(null, componentProp, previousSnap);

            var me = this;

            string fieldName = null;

            var hasNameAttribute = componentProp["x.Name"] != Script.Undefined;
            if (hasNameAttribute)
            {
                fieldName = componentProp["x.Name"].As<string>();

                Script.Write("delete componentProp['x.Name']");
            }

            var refHandlers = RefHandlers;
            Action<object> onRef = r =>
            {
                if (r == null)
                {
                    return;
                }

                var snaps = me.TypeScriptWrittenJsObject["snaps"];

                if (snaps == null)
                {
                    throw new InvalidOperationException("snaps not found");
                }

                snaps[snapKey] = r;
                if (fieldName != null)
                {
                    me.Caller[fieldName] = r;
                }

                if (refHandlers != null)
                {
                    foreach (var refHandler in refHandlers)
                    {
                        refHandler(r);
                    }
                }
            };

            componentProp[AttributeName.Ref] = onRef;

            if (data.CurrentComponentName == ComponentName.BInputMask.ToString())
            {
                // TODO: bug fix value null olduğunda _isCorrectFormatText metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.value] == null)
                {
                    componentProp[AttributeName.value] = "";
                }
            }

            if (data.CurrentComponentName == ComponentName.BComboBox.ToString())
            {
                // TODO: bug fix value null olduğunda organizeState metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.dataSource] == null)
                {
                    componentProp[AttributeName.dataSource] = new object[0];
                }
            }

            EvaluateBooleanValues(data.CurrentComponentName, data.CurrentComponentProp);
            EvaluateNumberValues(data.CurrentComponentName, data.CurrentComponentProp);
        }

        protected override void ProcessAttribute(string nodeName, string attributeName, string attributeValue, object prop, object elementProps)
        {
            BeforeStartToProcessAttribute(attributeName, attributeValue);

            elementProps[CurrentAttributeName] = EvaluateAttributeValue(CurrentAttributeValue, prop);

            var bindingInfo = BindingInfo.TryParseExpression(CurrentAttributeValue);

            if (bindingInfo != null)
            {
                BindSourceToTarget(bindingInfo, nodeName, prop);

                if (bindingInfo.BindingMode == BindingMode.TwoWay)
                {
                    var targetToSourceBinder = new TargetToSourceBinder
                    {
                        elementProps  = elementProps,
                        bindingInfo   = bindingInfo,
                        DataContext   = DataContext,
                        attributeName = CurrentAttributeName,
                        nodeName      = nodeName
                    };

                    targetToSourceBinder.TryBind();
                }
            }
        }

        static bool ComponentPropNeedToUpdate(string nodeName, dynamic component, string propName, object value)
        {
            if (nodeName == ComponentName.BInputMask.ToString() ||
                nodeName == ComponentName.BInput.ToString())
            {
                if (propName == ComponentPropName.value.ToString())
                {
                    var existingValue = component.state[propName];
                    if (existingValue == null || existingValue == string.Empty)
                    {
                        if (value == null || value as string == string.Empty)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        void BindSourceToTarget(BindingInfo bindingInfo, string nodeName, object source)
        {
            var currentAttributeName = CurrentAttributeName;

            Action<object> onRef = (dynamic componentt) =>
            {
                var component = componentt;
                Action UpdateTarget = () =>
                {
                    var value = bindingInfo.SourcePath.GetPropertyValue();

                    if (bindingInfo.Converter != null)
                    {
                        value = bindingInfo.Converter.Convert(value, null, bindingInfo.ConverterParameter, null);
                    }

                    var componentPropNeedToUpdate = ComponentPropNeedToUpdate(nodeName, component, currentAttributeName, value);
                    if (!componentPropNeedToUpdate)
                    {
                        return;
                    }

                    // TODO: move to function
                    if (nodeName == ComponentName.BInputMask.ToString() && currentAttributeName == ComponentPropName.value.ToString())
                    {
                        if (value == null)
                        {
                            value = "";
                        }
                    }

                    var newState = ObjectLiteral.Create<object>();
                    newState[currentAttributeName] = Unbox(value);
                    component.setState(newState);
                };

                bindingInfo.Source = source;

                bindingInfo.SourcePath.Listen(bindingInfo.Source, UpdateTarget);
            };

            AddToRefHandlers(onRef);
        }
        #endregion
    }
}