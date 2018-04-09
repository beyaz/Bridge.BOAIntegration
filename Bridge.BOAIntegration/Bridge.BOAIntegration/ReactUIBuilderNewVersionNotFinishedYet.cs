using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Bridge.BOAIntegration
{
    class UIBuilder
    {
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
        public void Create(string tagName, object attributes)
        {
            var constructorFunction = GetComponentClassByTagName(tagName);

            var propertyNames = GetOwnPropertyNames(attributes);
            for (var i = 0; i < propertyNames.Length; i++)
            {
                var propertyName  = propertyNames[i];
                var propertyValue = attributes[propertyName];

                var bindingInfoContract = propertyValue as BindingInfoContract;
                if (bindingInfoContract != null)
                {
                    var propertyPath = new PropertyPath(bindingInfoContract.SourcePath);
                    propertyPath.Walk(DataContext);
                    propertyValue = Unbox(propertyPath.GetPropertyValue());

                    attributes[propertyName] = propertyValue;
                }

                
            }

            var componentInfo = new ComponentInfo
            {
                ConstructorFunction = constructorFunction,
                Properties          = attributes
            };

            if (attributes.HasOwnProperty("innerHTML"))
            {
                componentInfo.Children = new[]
                {
                    new ComponentInfo
                    {
                        PureString = attributes["innerHTML"]
                    }
                };

                Script.Write(" delete attributes['innerHTML']");
            }

            Stack.Push(componentInfo);
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