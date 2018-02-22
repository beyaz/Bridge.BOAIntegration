using System;
using System.Windows;
using System.Windows.Data;
using Bridge.Html5;
using Bridge.jQuery2;

namespace Bridge.BOAIntegration
{
    public class ReactUIBuilderInput
    {
        #region Public Properties
        public object  Caller         { get; set; }
        public object  DataContext    { get; set; }
        public Element XmlRootElement { get; set; }
        public string  XmlUI          { get; set; }
        #endregion
    }

    public delegate object ComponentClassFinder(string nodeTagName);

    public class ReactUIBuilderData
    {
        #region Public Properties
        public string CurrentAttributeName  { get; set; }
        public string CurrentAttributeValue { get; set; }
        public object CurrentComponentClass { get; internal set; }
        public string CurrentComponentName  { get; set; }
        public object CurrentComponentProp  { get; internal set; }
        #endregion
    }

    public class PropsEvaluatedEventArgs : EventArgs
    {
        public object CurrentComponentClass { get; internal set; }
        public string CurrentComponentName  { get; internal set; }
        public object CurrentComponentProp  { get; internal set; }
    }

    public class ReactUIBuilder
    {
        #region Constants
        const string Comma = ",";
        #endregion

        #region Fields
        internal ReactUIBuilderInput Input;
        readonly ReactUIBuilderData  Data = new ReactUIBuilderData();

        object DataContext;
        #endregion

        #region Public Properties
        public ComponentClassFinder ComponentClassFinder { get; set; }

        public Action<ReactUIBuilderData>       OnBeforeStartToProcessAttribute { get; set; }

        public event  Action<PropsEvaluatedEventArgs> PropsEvaluated;

        
        #endregion

        #region Public Methods
        public ReactElement Build(ReactUIBuilderInput input)
        {
            Input = input;

            DataContext = input.DataContext;

            var rootNode = input.XmlRootElement ?? GetRootNode(input.XmlUI);

            return BuildNodes(rootNode, "0", null).As<ReactElement>();
        }
        #endregion

        #region Methods
        internal static object BuildNodeAsText(string innerText, object dataContext)
        {
            var bindingInfo = BindingInfo.TryParseExpression(innerText);

            if (bindingInfo == null)
            {
                if (string.IsNullOrWhiteSpace(innerText))
                {
                    return null;
                }

                return innerText;
            }

            var propertyPath = bindingInfo.SourcePath;

            propertyPath.Walk(dataContext);

            return propertyPath.GetPropertyValue();
        }

        internal object EvaluateAttributeValue(string attributeValue, object prop)
        {
            var isMethod = attributeValue.StartsWith("this.");
            if (isMethod)
            {
                // ReSharper disable once UnusedVariable
                var methodName = attributeValue.RemoveFromStart("this.").Trim();

                // ReSharper disable once UnusedVariable
                var caller = Input.Caller;

                return Script.Write<object>(@"function(){ return caller[methodName].apply(caller,arguments);  } ");
            }

            var bindingInfo = BindingInfo.TryParseExpression(attributeValue);

            if (bindingInfo != null)
            {
                var propertyPath = bindingInfo.SourcePath;

                propertyPath.Walk(prop);

                return Unbox(propertyPath.GetPropertyValue());
            }

            return attributeValue;
        }

        static Element GetRootNode(string xmlString)
        {
            if (xmlString == null)
            {
                throw new ArgumentNullException(nameof(xmlString));
            }

            try
            {
                return jQuery.ParseXML(xmlString.Trim()).FirstChild.As<Element>();
            }
            catch (Exception e)
            {
                throw new SystemException("XmlParseErrorOccured.", e);
            }
        }

        [Template("Bridge.unbox({0},true)")]
        static extern object Unbox(object o);

        void BeforeStartToProcessAttribute(string attributeName, string attributeValue)
        {
            Data.CurrentAttributeName  = attributeName;
            Data.CurrentAttributeValue = attributeValue?.Trim();

            if (OnBeforeStartToProcessAttribute == null)
            {
                return;
            }

            OnBeforeStartToProcessAttribute(Data);
        }

        void BInputMask_onChange_Handler(string value, string bindingPath)
        {
            var propertyPath = new PropertyPath(bindingPath);

            propertyPath.Walk(DataContext);

            propertyPath.SetPropertyValue(value);
        }

        object[] BuildChildNodes(Element node, string nodeLocation, object componentProp)
        {
            var childNodes = node.ChildNodes;
            var len        = childNodes.Length;

            var childElements = new object[0];

            for (var i = 0; i < len; i++)
            {
                var childElement = BuildNodes(childNodes[i].As<Element>(), nodeLocation + Comma + i, componentProp);
                if (childElement == null)
                {
                    continue;
                }

                childElements.Push(childElement);
            }

            return childElements;
        }

        ReactElement BuildNodeAsParentComponentProperty(Element node, string nodeLocation, object parentComponentProp, string parentNodeName, string nodeName)
        {
            var value = BuildChildNodes(node, nodeLocation, parentComponentProp);

            var propertyName = nodeName.RemoveFromStart(parentNodeName + ".");

            BeforeStartToProcessAttribute(propertyName, null);

            parentComponentProp[Data.CurrentAttributeName] = value;

            return null;
        }

        object BuildNodes(Element node, string nodeLocation, object parentComponentProp)
        {
            if (node.NodeType == NodeType.Text)
            {
                return BuildNodeAsText(node.GetInnerText(), DataContext);
            }

            if (node.TagName == "ComboBoxColumn")
            {
                return EvaluateProps(node.TagName, node, DataContext, nodeLocation);
            }

            var parentNodeName = node.ParentNode?.NodeName;
            var nodeName       = node.NodeName;

            var isParentComponentProperty = nodeName.StartsWith(parentNodeName + ".");
            if (isParentComponentProperty)
            {
                return BuildNodeAsParentComponentProperty(node, nodeLocation, parentComponentProp, parentNodeName, nodeName);
            }

            var componentConstructor = GetComponentClassByTagName(node.TagName);

            if (node.HasChildNodes() == false)
            {
                return ReactElement.Create(componentConstructor, EvaluateProps(componentConstructor, node, DataContext, nodeLocation));
            }

            var componentProp = EvaluateProps(componentConstructor, node, DataContext, nodeLocation);

            return ReactElement.Create(componentConstructor, componentProp, BuildChildNodes(node, nodeLocation, componentProp));
        }

        object EvaluateProps(object componentConstructor, Element node, object prop, string nodeLocation)
        {
            var attributes = node.Attributes;
            var len        = attributes.Length;

            var elementProps = ObjectLiteral.Create<object>();

            for (var i = 0; i < len; i++)
            {
                var attribute = attributes[i];
                ProcessAttribute(node.TagName, attribute.NodeName, attribute.NodeValue, prop, elementProps);
            }

            if (elementProps[AttributeName.key] == Script.Undefined)
            {
                elementProps[AttributeName.key] = nodeLocation;
            }

            if (PropsEvaluated != null)
            {
                var propsEvaluatedEventArgs = new PropsEvaluatedEventArgs
                {
                    CurrentComponentName  = node.NodeName,
                    CurrentComponentClass = componentConstructor,
                    CurrentComponentProp  = elementProps
                };

                PropsEvaluated(propsEvaluatedEventArgs);
                
            }

            return elementProps;
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
                    throw new ArgumentNullException("ComponentClassFinder returned null value.");
                }

                return componentClass;
            }

            throw new NotImplementedException(nodeTagName);
        }

        void ProcessAttribute(string nodeName, string attributeName, string attributeValue, object prop, object elementProps)
        {
            BeforeStartToProcessAttribute(attributeName, attributeValue.Trim());

            attributeName  = Data.CurrentAttributeName;
            attributeValue = Data.CurrentAttributeValue;

            elementProps[attributeName] = EvaluateAttributeValue(attributeValue, prop);

            var bindingInfo = BindingInfo.TryParseExpression(attributeValue);

            if (bindingInfo != null && bindingInfo.BindingMode == BindingMode.TwoWay)
            {
                var targetToSourceBinder = new TargetToSourceBinder
                {
                    elementProps  = elementProps,
                    bindingInfo   = bindingInfo,
                    DataContext   = DataContext,
                    attributeName = attributeName,
                    nodeName      = nodeName
                };

                targetToSourceBinder.TryBind();
            }
        }
        #endregion

        
    }
}