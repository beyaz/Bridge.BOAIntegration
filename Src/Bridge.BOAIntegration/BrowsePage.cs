using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BOA.Common.Types;
using BOA.Messaging;
using BOA.UI.CardGeneral.DebitCard.Common.Data;

namespace Bridge.BOAIntegration
{

    public static class BrowsePageExtensions
    {
        static BindingFlags AllBindings
        {
            get { return BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic; }
        }

        /// <summary>
        ///     Tries the type of the get proper.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Type TryGetProperType(this Type type, string propertyName)
        {
            if (type == null)
            {
                return null;
            }
            var property = type.GetProperty(propertyName, AllBindings);
            if (property == null)
            {
                return null;
            }

            return property.PropertyType;
        }


        public static void ConfigureColumns(this BrowsePage browseForm, IEnumerable<DataGridColumnInfoContract> columns)
        {

            var fields = new object[0];
            foreach (var item in columns)
            {
                var field = new object
                {
                   ["key"] = item.BindingPath,
                    ["name"] =item.Label,
                    ["resizable"] = true



                };

                if (item.DataType?.IsNumeric() == true)
                {
                    field["type"] = "number";
                    field["numberFormat"] = "M";
                }

                fields.Push(field);
            }


            browseForm[AttributeName.DolarColumns] = fields;

        }

    }

    public class BrowsePage : BasePage
    {

        #region Public Properties
        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        public BState State { [Template("$TypeScriptVersion.state")] get; }
        #endregion

        #region Public Methods
        public virtual string GetMessage(string groupName, string propertyName)
        {
            return MessagingHelper.GetMessage(groupName, propertyName);
        }

        public void SetState<T>(T state) where T : BState
        {
            setState(state);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void ShowError(string message, Result[] results)
        {
            var dialogHelper = NodeModules.BDialogHelper();

            Script.Write("dialogHelper.showError(this.$TypeScriptVersion.state.context,message,results); ");
        }
        #endregion

        #region Methods
        protected ReactElement BuildUI(string xmlUI, object prop)
        {
            var reactUiBuilder = new ReactUIBuilder
            {
                ComponentClassFinder = NodeModules.FindComponent,
                OnPropsEvaluated     = OnPropsEvaluated,
                OnBeforeStartToProcessAttribute = OnBeforeStartToProcessAttribute
            };

            return reactUiBuilder.Build(new ReactUIBuilderInput
            {
                XmlUI  = xmlUI,
                DataContext   = prop,
                Caller = this
            });
        }

        void OnBeforeStartToProcessAttribute(ReactUIBuilderData data)
        {
            MakeLowercaseFirstChar(data);
        }

         static void MakeLowercaseFirstChar(ReactUIBuilderData data)
        {
            data.CurrentAttributeName = data.CurrentAttributeName[0].ToString().ToLower() + data.CurrentAttributeName.Substring(1);
        }

        object OnPropsEvaluated(ReactUIBuilderData data)
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

            componentProp[AttributeName.Ref] = Script.Write<object>("function(r){  me.$TypeScriptVersion.snaps[ snapKey ] = r;  }");

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



            return componentProp;
        }

        [Template("$TypeScriptVersion.setState({0})")]
        extern void setState(object state);

        [Template("$TypeScriptVersion.forceUpdate()")]
        protected extern void forceUpdate(); 
        #endregion
    }
}