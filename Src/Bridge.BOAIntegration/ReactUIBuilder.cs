﻿using System;
using System.Windows;
using System.Windows.Data;
using Bridge.Html5;
using Bridge.jQuery2;

namespace Bridge.BOAIntegration
{
    public class ReactUIBuilderInput
    {
        #region Public Properties
        public object Caller      { get; set; }
        public object DataContext { get; set; }
        public string XmlUI       { get; set; }
        #endregion
    }

    public delegate object ComponentClassFinder(string nodeTagName);

    public class ReactUIBuilderData
    {
        #region Public Properties
        public string CurrentAttributeName  { get; set; }
        public string CurrentAttributeValue { get; set; }
        public object CurrentComponentClass { get; internal set; }
        public object CurrentComponentProp  { get; internal set; }
        public string CurrentComponentName { get; set; }
        #endregion
    }

    public class ReactUIBuilder
    {
        #region Constants
        const string Comma = ",";
        #endregion

        #region Fields
        readonly ReactUIBuilderData Data = new ReactUIBuilderData();
        ReactUIBuilderInput         Input;
        #endregion

        #region Public Properties
        public ComponentClassFinder ComponentClassFinder { get; set; }

        public Action<ReactUIBuilderData>       OnBeforeStartToProcessAttribute { get; set; }
        public Func<ReactUIBuilderData, object> OnPropsEvaluated                { get; set; }
        #endregion

        #region Public Methods
        public ReactElement Build(ReactUIBuilderInput input)
        {
            Input = input;
            var rootNode = GetRootNode(input.XmlUI);
            return BuildNodes(rootNode, input.DataContext, "0", null);
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

        void BDateTimePicker_onChange_Handler(DateTime? value, string bindingPath)
        {
            var caller       = Input.Caller;
            var propertyPath = new PropertyPath(bindingPath);
            var state        = caller[AttributeName.state];

            propertyPath.Walk(state);

            propertyPath.SetPropertyValue(value.As<object>());
        }

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

        object[] BuildChildNodes(Element node, object prop, string nodeLocation, object componentProp)
        {
            var childNodes = node.ChildNodes;
            var len        = childNodes.Length;

            var childElements = new object[0];

            for (var i = 0; i < len; i++)
            {
                var childElement = BuildNodes(childNodes[i].As<Element>(), prop, nodeLocation + Comma + i, componentProp);
                if (childElement == null)
                {
                    continue;
                }

                childElements.Push(childElement);
            }

            return childElements;
        }

        ReactElement BuildNodes(Element node, object prop, string nodeLocation, object parentComponentProp)
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

            if (node.TagName == "ComboBoxColumn")
            {
                return EvaluateProps(node.TagName, node, prop, nodeLocation).As<ReactElement>();
            }

            var parentNodeName = node.ParentNode?.NodeName;
            var nodeName       = node.NodeName;

            if (nodeName.StartsWith(parentNodeName + "."))
            {
                var value = BuildChildNodes(node, prop, nodeLocation, parentComponentProp);

                var propertyName = nodeName.RemoveFromStart(parentNodeName + ".");

                BeforeStartToProcessAttribute(propertyName, null);

                parentComponentProp[Data.CurrentAttributeName] = value;

                return null;
            }

            var componentConstructor = GetComponentClassByTagName(node.TagName);

            if (node.HasChildNodes() == false)
            {
                return ReactElement.Create(componentConstructor, EvaluateProps(componentConstructor, node, prop, nodeLocation));
            }

            var componentProp = EvaluateProps(componentConstructor, node, prop, nodeLocation);

            return ReactElement.Create(componentConstructor, componentProp, BuildChildNodes(node, prop, nodeLocation, componentProp));
        }

        object EvaluateAttributeValue(string attributeValue, object prop)
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

                return propertyPath.GetPropertyValue();
            }

            return attributeValue;
        }

        object EvaluateProps(object componentConstructor, Element node, object prop, string nodeLocation)
        {
            // ReSharper disable once UnusedVariable
            var me = this;

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

            if (OnPropsEvaluated != null)
            {
                Data.CurrentComponentName = node.NodeName;
                Data.CurrentComponentClass = componentConstructor;
                Data.CurrentComponentProp  = elementProps;

                elementProps = OnPropsEvaluated(Data);
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

            var name  = Data.CurrentAttributeName;
            var value = Data.CurrentAttributeValue;

            elementProps[name] = EvaluateAttributeValue(value, prop);

            var bindingInfo = BindingInfo.TryParseExpression(value);

            if (bindingInfo != null && bindingInfo.BindingMode == BindingMode.TwoWay)
            {
                // ReSharper disable once UnusedVariable
                var bindingPath = bindingInfo.SourcePath.Path;

                if (nodeName == "BDateTimePicker")
                {
                    var onChangeHandlerFunction = Script.Write<object>(@"function(p0,value)
                    {
                            me.BDateTimePicker_onChange_Handler(value,bindingPath);
                    }");
                    elementProps["onChange"] = onChangeHandlerFunction;
                }
            }
        }
        #endregion
    }
}