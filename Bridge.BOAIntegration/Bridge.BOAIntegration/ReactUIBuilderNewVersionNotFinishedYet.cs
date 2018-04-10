using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using BOA.Messaging;

namespace Bridge.BOAIntegration
{

    class UIBuilderForBOA: UIBuilder
    {
        

        protected override void ProcessProperty(object elementProps, string propertyName)
        {
            var propertyValue = elementProps[propertyName] as string;

            if (MessagingResolver.IsMessagingExpression(propertyValue))
            {
                var pair = MessagingResolver.GetMessagingExpressionValue(propertyValue);

                elementProps[propertyName] =  MessagingHelper.GetMessage(pair.Key, pair.Value);
                return;
            }

            base.ProcessProperty(elementProps,propertyName);

        }
    }
    class UIBuilder
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