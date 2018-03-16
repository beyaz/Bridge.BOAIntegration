using System;
using System.Collections.Generic;

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
            {"BAccountComponent", new[] {"isVisibleBalance", "isVisibleIBAN"}},
            {"BComboBox", new[] {"multiSelect", "multiColumn", "isAllOptionIncluded"}},
            {"BInput", new[] {"noWrap", "multiLine"}},
            {"BParameterComponent", new[] {"disabled"}}
        };

        static readonly Dictionary<string, string[]> NumberAttributes = new Dictionary<string, string[]>
        {
            {"BCheckBox", new[] {"size"}},
            {"BComboBox", new[] {"size"}},
            {"BInput", new[] {"rows", "rowsMax", "size"}},
            {"BParameterComponent", new[] {"size"}}
        };
        #endregion

        #region Fields
        internal object TypeScriptWrittenJsObject;
        #endregion

        #region Constructors
        public ReactUIBuilderBOAVersion()
        {
            ComponentClassFinder   =  NodeModules.FindComponent;
            PropsEvaluated         += OnPropsEvaluated;
            BeforeProcessAttribute += OnBeforeStartToProcessAttribute;
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

        static void MakeLowercaseFirstChar(BeforeStartToProcessAttributeEventArgs data)
        {
            data.CurrentAttributeName = data.CurrentAttributeName[0].ToString().ToLower() + data.CurrentAttributeName.Substring(1);
        }

        static void OnBeforeStartToProcessAttribute(BeforeStartToProcessAttributeEventArgs data)
        {
            MakeLowercaseFirstChar(data);
        }

        void OnPropsEvaluated(PropsEvaluatedEventArgs data)
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

            Action<object> onRef = (r) =>
            {
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
            };


            componentProp[AttributeName.Ref] = onRef;

            if (data.CurrentComponentName == "BInputMask")
            {
                // TODO: bug fix value null olduğunda _isCorrectFormatText metodu patlıyor. düzeltileiblir
                if (componentProp[AttributeName.value] == null)
                {
                    componentProp[AttributeName.value] = "";
                }
            }

            if (data.CurrentComponentName == "BComboBox")
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
        #endregion
    }
}