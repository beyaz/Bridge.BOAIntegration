using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using BOA.Messaging;

namespace Bridge.BOAIntegration
{

    public  class UIBuilderForBOA : UIBuilder
    {
        public UIBuilderForBOA()
        {
            ComponentClassFinder = NodeModules.FindComponent;
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

        protected override void OnComponentInfoCreated(ComponentInfo info)
        {

            var componentProp = info.Properties;

            var propertyNames = GetOwnPropertyNames(componentProp);
            var len           = propertyNames.Length;

            for (var i = 0; i < len; i++)
            {
                var propertyName  = propertyNames[i];
                var propertyValue = componentProp[propertyName];

                var propertyValueAsString = propertyValue as string;
                if (MessagingResolver.IsMessagingExpression(propertyValueAsString))
                {
                    var pair = MessagingResolver.GetMessagingExpressionValue(propertyValueAsString);

                    componentProp[propertyName] = MessagingHelper.GetMessage(pair.Key, pair.Value);
                    continue;
                }


                var bindingInfoContract = propertyValue as BindingInfoContract;
                if (bindingInfoContract != null)
                {
                    var bindingInfo = bindingInfoContract.ToBindingInfo();

                    BindSourceToTarget(bindingInfo, info.NodeName, DataContext, propertyName);

                    if (bindingInfo.BindingMode == BindingMode.TwoWay)
                    {
                        var targetToSourceBinder = new TargetToSourceBinder
                        {
                            elementProps  = componentProp,
                            bindingInfo   = bindingInfo,
                            DataContext   = DataContext,
                            attributeName = propertyName,
                            nodeName      = info.NodeName
                        };

                        targetToSourceBinder.TryBind();
                    }

                    componentProp[propertyName] = ReadValue(DataContext, bindingInfoContract.SourcePath);
                }
            }


            var pageParams = State.PageParams;
            var context = State.Context;



            var componentName = info.NodeName;
          

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

            info.Properties = componentProp = JsLocation._extend.Apply(null, componentProp, previousSnap);

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


        }
    }

    public class UIBuilder
    {
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
        public   ComponentClassFinder ComponentClassFinder{ get; set; }
    public   object TypeScriptWrittenJsObject { get; set; }
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


        protected static object ReadValue(object dataContext, string propertyPath)
        {
            var path = new PropertyPath(propertyPath);
            path.Walk(dataContext);
            return Unbox(path.GetPropertyValue());
        }



        void ResolveBindingValues(ComponentInfo info)
        {
            var elementProps = info.Properties;

            var propertyNames = GetOwnPropertyNames(elementProps);
            var len           = propertyNames.Length;

            for (var i = 0; i < len; i++)
            {
                var propertyName = propertyNames[i];
                var propertyValue = elementProps[propertyName];


                var bindingInfoContract = propertyValue as BindingInfoContract;
                if (bindingInfoContract != null)
                {
                    elementProps[propertyName] = ReadValue(DataContext, bindingInfoContract.SourcePath);
                }
            }
        }

        public void Create(string tagName, object componentProps)
        {
            var constructorFunction = GetComponentClassByTagName(tagName);

            if (componentProps[AttributeName.key] == Script.Undefined)
            {
                componentProps[AttributeName.key] = GetNextKey();
            }

            var componentInfo = new ComponentInfo
            {
                ConstructorFunction = constructorFunction,
                Properties          = componentProps,
                NodeName = tagName
            };

            OnComponentInfoCreated(componentInfo);

            if (componentProps.HasOwnProperty("innerHTML"))
            {
                componentInfo.Children = new[]
                {
                    new ComponentInfo
                    {
                        PureString = componentProps["innerHTML"]
                    }
                };

                Script.Write(" delete componentProps['innerHTML']");
            }

            RefHandlers = null;

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

        

        protected virtual void OnComponentInfoCreated(ComponentInfo info)
        {
            ResolveBindingValues(info);
        }
    }
    public class ComponentInfo
    {
        #region Fields
        internal ComponentInfo[] Children;
        internal object          ConstructorFunction;
        internal object          Properties;
        internal object          PureString;
        internal string NodeName;
        #endregion
    }
}