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
            {"BComboBox", new[] {"multiSelect", "multiColumn", "isAllOptionIncluded"}}
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

            componentProp[AttributeName.snapKey]    = snapKey;
            componentProp[AttributeName.pageParams] = pageParams;
            componentProp[AttributeName.context]    = context;
            componentProp[AttributeName.snapshot]   = State[AttributeName.snapshot][snapKey];
            var previousSnap = State[AttributeName.dynamicProps][snapKey];

            componentProp = JsLocation._extend.Apply(null, componentProp, previousSnap);

            // ReSharper disable once UnusedVariable
            var me = this;

            componentProp[AttributeName.Ref] = Script.Write<object>("function(r){  me.TypeScriptWrittenJsObject.snaps[ snapKey ] = r;  }");

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
        }
        #endregion
    }
}