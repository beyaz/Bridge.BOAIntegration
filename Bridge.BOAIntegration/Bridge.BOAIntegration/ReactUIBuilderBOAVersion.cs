using System;
using System.Windows.Data;
using BOA.Messaging;

namespace Bridge.BOAIntegration
{
    class ReactUIBuilderBOAVersion : ReactUIBuilder
    {
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
            var booleanAttributes = MapHelper.GetBooleanAttributes(componentName);

            if (booleanAttributes == null)
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

                if (stringValue.ToUpper() == "FALSE")
                {
                    componentProp[attributeName] = false.As<object>();
                    continue;
                }

                if (stringValue.ToUpper() == "TRUE")
                {
                    componentProp[attributeName] = true.As<object>();
                    continue;
                }

                throw new ArgumentException($"{componentName} -> {attributeName} must be boolan (false/true)");
            }
        }

        internal static void EvaluateNumberValues(string componentName, object componentProp)
        {
            var attributes = MapHelper.GetNumberAttributes(componentName);

            if (attributes == null)
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

                var intValue = int.Parse(stringValue);

                componentProp[attributeName] = intValue.As<object>();
            }
        }

        protected internal override object EvaluateAttributeValue(string attributeValue, object prop)
        {
            if (MessagingResolver.IsMessagingExpression(attributeValue))
            {
                var pair = MessagingResolver.GetMessagingExpressionValue(attributeValue);

                return MessagingHelper.GetMessage(pair.Key, pair.Value);
            }

            return base.EvaluateAttributeValue(attributeValue, prop);
        }

        protected override void BeforeStartToProcessAttribute(string attributeName, string attributeValue)
        {
            base.BeforeStartToProcessAttribute(attributeName, attributeValue);

            // MakeLowercaseFirstChar
            CurrentAttributeName = CurrentAttributeName[0].ToString().ToLower() + CurrentAttributeName.Substring(1);
        }

        protected override void OnPropsEvaluated( PropsEvaluatedEventArgs data)
        {
            var componentProp = data.CurrentComponentProp;
            var componentName = data.CurrentComponentName;

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

            
            if (componentName == ComponentName.BInputMask.ToString())
            {
                // TODO: bug fix value null olduğunda _isCorrectFormatText metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.value] == null)
                {
                    componentProp[AttributeName.value] = "";
                }
            }

            if (componentName == ComponentName.BComboBox.ToString())
            {
                // TODO: bug fix value null olduğunda organizeState metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.dataSource] == null)
                {
                    componentProp[AttributeName.dataSource] = new object[0];
                }
            }

            EvaluateBooleanValues(componentName, componentProp);
            EvaluateNumberValues(componentName, componentProp);
        }

        protected override void ProcessAttribute(string nodeName, string attributeName, string attributeValue, object prop, object elementProps)
        {
            BeforeStartToProcessAttribute(attributeName, attributeValue);

            elementProps[CurrentAttributeName] = EvaluateAttributeValue(CurrentAttributeValue, prop);

            var bindingInfo = BindingExpressionParser.TryParse(CurrentAttributeValue).ToBindingInfo();

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