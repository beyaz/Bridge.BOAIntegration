using System;
using System.Windows.Data;
using Bridge.Html5;
using Bridge.jQuery2;

namespace Bridge.BOAIntegration
{
    public class ReactUIBuilderInput
    {
        public string xmlUi;
        public object prop;
    }
    public class ReactUIBuilder
    {
        #region Constants
        const string Comma = ",";
        #endregion

        #region Public Properties
        public Func<string, object>         ComponentClassFinder { get; set; }
        public Func<object, object, object> OnPropsEvaluated     { get; set; }
        #endregion

        #region Public Methods
        public ReactElement Build(ReactUIBuilderInput input)
        {
            var rootNode = GetRootNode(input.xmlUi);
            return BuildNodes(rootNode, input.prop, "0");
        }
        #endregion

        #region Methods
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

        object BuildChildNodes(Element node, object prop, string nodeLocation)
        {
            var childNodes = node.ChildNodes;
            var len        = childNodes.Length;

            var childElements = new object[0];

            for (var i = 0; i < len; i++)
            {
                var childElement = BuildNodes(childNodes[i].As<Element>(), prop, nodeLocation + Comma + i);
                if (childElement == null)
                {
                    continue;
                }

                childElements.Push(childElement);
            }

            return childElements;
        }

        ReactElement BuildNodes(Element node, object prop, string nodeLocation)
        {
            if (node.NodeType == NodeType.Text)
            {
                var innerText   = node.GetInnerText();
                var bindingInfo = BindingInfo.TryParseExpression(innerText);

                if (bindingInfo == null)
                {
                    if (string.IsNullOrWhiteSpace(innerText))
                    {
                        return null;
                    }

                    return innerText.As<ReactElement>();
                }

                var propertyPath = bindingInfo.SourcePath;

                propertyPath.Walk(prop);
                return propertyPath.GetPropertyValue().As<ReactElement>();
            }

            var componentConstructor = GetComponentClassByTagName(node.TagName);

            if (node.HasChildNodes() == false)
            {
                return ReactElement.Create(componentConstructor, EvaluateProps(componentConstructor, node, prop, nodeLocation));
            }

            return ReactElement.Create(componentConstructor, EvaluateProps(componentConstructor, node, prop, nodeLocation), BuildChildNodes(node, prop, nodeLocation));
        }

        object EvaluateProps(object componentConstructor, Element node, object prop, string nodeLocation)
        {
            var attributes = node.Attributes;
            var len        = attributes.Length;

            var elementProps = ObjectLiteral.Create<object>();

            for (var i = 0; i < len; i++)
            {
                var attribute = attributes[i];

                var    name          = attribute.NodeName;
                var    value         = attribute.NodeValue;

                elementProps[name] = EvaluateAttributeValue(value, prop);
            }

            if (elementProps[AttributeName.key] == Script.Undefined)
            {
                elementProps[AttributeName.key] = nodeLocation;
            }

            elementProps = OnPropsEvaluated?.Invoke(componentConstructor, elementProps);

            return elementProps;
        }

        object EvaluateAttributeValue( string attributeValue, object prop)
        {
            var isMethod = attributeValue.StartsWith("this.");
            if (isMethod)
            {

                
            }

            var bindingInfo = BindingInfo.TryParseExpression(attributeValue);

            if (bindingInfo != null)
            {
                var propertyPath = bindingInfo.SourcePath;

                propertyPath.Walk(prop);

                return propertyPath.GetPropertyValue();
            }

            return attributeValue;

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
        #endregion
    }
}