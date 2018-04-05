using System;
using System.Collections.Generic;

namespace Bridge.BOAIntegration
{
    class UIBuilder
    {
        #region Fields
        readonly ComponentClassFinder ComponentClassFinder;
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

            var componentInfo = new ComponentInfo
            {
                ConstructorFunction = constructorFunction,
                Properties          = attributes
            };

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

            if (topComponentInfo.children == null)
            {
                topComponentInfo.children = new ComponentInfo[0];
            }

            topComponentInfo.children.Push(componentInfo);
        }
        #endregion

        #region Methods
        ReactElement ConvertToReactElement(ComponentInfo componentInfo)
        {
            if (componentInfo == null)
            {
                throw new ArgumentNullException(nameof(componentInfo));
            }

            var children = componentInfo.children;

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
            internal ComponentInfo[] children;
            internal object          ConstructorFunction;
            internal object          Properties;
            #endregion
        }
    }
}