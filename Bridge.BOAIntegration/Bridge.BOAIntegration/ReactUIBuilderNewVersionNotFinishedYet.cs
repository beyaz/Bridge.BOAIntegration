using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using BOA.Messaging;

namespace Bridge.BOAIntegration
{

    class UIBuilderForBOA: UIBuilder
    {

        void  ProcessBindingInfo(BindingInfo bindingInfo,string nodeName, object prop, string currentAttributeName, object elementProps)
        {
            BindSourceToTarget(bindingInfo, nodeName, prop, currentAttributeName);

            if (bindingInfo.BindingMode == BindingMode.TwoWay)
            {
                var targetToSourceBinder = new TargetToSourceBinder
                {
                    elementProps  = elementProps,
                    bindingInfo   = bindingInfo,
                    DataContext   = DataContext,
                    attributeName = currentAttributeName,
                    nodeName      = nodeName
                };

                targetToSourceBinder.TryBind();
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

        void BindSourceToTarget(BindingInfo bindingInfo, string nodeName, object source,string currentAttributeName)
        {

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


        BState State => TypeScriptWrittenJsObject[AttributeName.state].As<BState>();

        protected override void ProcessProperty(object elementProps, string propertyName)
        {
            var propertyValue = elementProps[propertyName] as string;

            if (MessagingResolver.IsMessagingExpression(propertyValue))
            {
                var pair = MessagingResolver.GetMessagingExpressionValue(propertyValue);

                elementProps[propertyName] =  MessagingHelper.GetMessage(pair.Key, pair.Value);
                return;
            }

            var bindingInfo = BindingExpressionParser.TryParse(propertyValue).ToBindingInfo();
            if (bindingInfo != null)
            {
                ProcessBindingInfo(bindingInfo,"?","?","?","?");
                return;
            }

            base.ProcessProperty(elementProps,propertyName);

        }

        protected override object OnPropsEvaluated(string componentName, object componentProp)
        {
            

            var pageParams = State.PageParams;
            var context = State.Context;

            var snapKey = componentProp[AttributeName.key].As<string>();
            if (snapKey == null)
            {
                throw new InvalidOperationException(nameof(snapKey) + " not found.");
            }

            componentProp[AttributeName.snapKey] = snapKey;
            componentProp[AttributeName.pageParams] = pageParams;
            componentProp[AttributeName.context] = context;
            componentProp[AttributeName.snapshot] = State[AttributeName.snapshot][snapKey];
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

            return componentProp;
        }

         static void EvaluateNumberValues(string componentName, object componentProp)
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

         static void EvaluateBooleanValues(string componentName, object componentProp)
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





    }
    class UIBuilder
    {
        protected virtual object OnPropsEvaluated(string componentName, object componentProp)
        {
            return componentProp;
        }

        public int RenderCount { get; set; }
        protected Action<object>[] RefHandlers;

        protected void AddToRefHandlers(Action<object> item)
        {
            if (RenderCount > 1)
            {
                return;
            }

            if (RefHandlers == null)
            {
                RefHandlers = new Action<object>[0];
            }

            RefHandlers.Push(item);
        }

        #region Fields
        public   ComponentClassFinder ComponentClassFinder;
        public   object               TypeScriptWrittenJsObject;
        readonly Stack<ComponentInfo> Stack = new Stack<ComponentInfo>();
        #endregion

        #region Constructors
        public UIBuilder()
        {
            ComponentClassFinder = NodeModules.FindComponent;
        }
        #endregion

        #region Public Properties
        public object Caller      { get; set; }
        public object DataContext { get; set; }
        public object Result      { get; private set; }
        #endregion

        #region Public Methods


        protected virtual void ProcessProperty(object elementProps, string propertyName)
        {
            var propertyValue = elementProps[propertyName];

            var bindingInfoContract = propertyValue as BindingInfoContract;
            if (bindingInfoContract != null)
            {
                var propertyPath = new PropertyPath(bindingInfoContract.SourcePath);
                propertyPath.Walk(DataContext);
                propertyValue = Unbox(propertyPath.GetPropertyValue());

                elementProps[propertyName] = propertyValue;
            }
        }

        public void Create(string tagName, object elementProps)
        {
            var constructorFunction = GetComponentClassByTagName(tagName);

            var propertyNames = GetOwnPropertyNames(elementProps);
            var len = propertyNames.Length;

            for (var i = 0; i < len; i++)
            {
                ProcessProperty(elementProps, propertyNames[i]);
            }

            if (elementProps[AttributeName.key] == Script.Undefined)
            {
                elementProps[AttributeName.key] = GetNextKey();
            }

            elementProps = OnPropsEvaluated(tagName,elementProps);


            var componentInfo = new ComponentInfo
            {
                ConstructorFunction = constructorFunction,
                Properties          = elementProps
            };

            if (elementProps.HasOwnProperty("innerHTML"))
            {
                componentInfo.Children = new[]
                {
                    new ComponentInfo
                    {
                        PureString = elementProps["innerHTML"]
                    }
                };

                Script.Write(" delete attributes['innerHTML']");
            }

            Stack.Push(componentInfo);
        }

        int _key;
        string GetNextKey()
        {
            _key++;
            return "cmp"+_key;
        }


        public void EndOf()
        {
            var stackCount = Stack.Count;

            if (stackCount == 0)
            {
                throw new InvalidOperationException();
            }

            if (stackCount == 1)
            {
                var resultComponentInfo = Stack.Pop();
                Result = ConvertToReactElement(resultComponentInfo);
                return;
            }

            var componentInfo = Stack.Pop();

            var topComponentInfo = Stack.Peek();

            if (topComponentInfo.Children == null)
            {
                topComponentInfo.Children = new ComponentInfo[0];
            }

            topComponentInfo.Children.Push(componentInfo);
        }
        #endregion

        #region Methods
        [Template("Bridge.unbox({0},true)")]
        protected static extern object Unbox(object o);

        ReactElement ConvertToReactElement(ComponentInfo componentInfo)
        {
            if (componentInfo == null)
            {
                throw new ArgumentNullException(nameof(componentInfo));
            }

            if (componentInfo.PureString != null)
            {
                return componentInfo.PureString.As<ReactElement>();
            }

            var children = componentInfo.Children;

            if (children == null)
            {
                return ReactElement.Create(componentInfo.ConstructorFunction, componentInfo.Properties);
            }

            var subElements = new object[0];

            var len = children.Length;
            for (var i = 0; i < len; i++)
            {
                var info = children[i];

                var childElement = ConvertToReactElement(info);

                subElements.Push(childElement);
            }

            return ReactElement.Create(componentInfo.ConstructorFunction, componentInfo.Properties, subElements);
        }

        object GetComponentClassByTagName(string nodeTagName)
        {
            if (nodeTagName == "div")
            {
                return nodeTagName;
            }

            if (ComponentClassFinder != null)
            {
                var componentClass = ComponentClassFinder(nodeTagName);

                if (componentClass == null)
                {
                    throw new ArgumentNullException("ComponentClassFinder returned null value.->" + nodeTagName);
                }

                return componentClass;
            }

            throw new NotImplementedException(nodeTagName);
        }
        #endregion

        class ComponentInfo
        {
            #region Fields
            internal ComponentInfo[] Children;
            internal object          ConstructorFunction;
            internal object          Properties;
            internal object          PureString;
            #endregion
        }
    }
}