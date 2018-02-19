﻿using System;
using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.QUnit;

namespace Bridge.BOAIntegration
{
    [ObjectLiteral]
    class Component_1_Prop
    {
        #region Public Properties
        public string Name5 { get; set; }
        #endregion
    }

    class Component_1 : Component
    {
        #region Properties
        Component_1_Prop Props
        {
            get { return this["props"].As<Component_1_Prop>(); }
        }
        #endregion

        #region Public Methods
        public override ReactElement render()
        {
            var prop = ObjectLiteral.Create<object>();
            prop["href"] = Props.Name5;

            return ReactElement.Create("a", prop);
        }
        #endregion
    }

    class AllTest
    {
        #region Public Methods
        public static void Register()
        {
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(ShouldRenderSimpleOneDivInnerHTML), ShouldRenderSimpleOneDivInnerHTML);
        }
        #endregion

        #region Methods
        static dynamic BuildUI(string xmlUI, dynamic prop)
        {
            var builder = new ReactUIBuilder
            {
                ComponentClassFinder = tag =>
                {
                    if (tag == nameof(Component_1))
                    {
                        return typeof(Component_1);
                    }

                    return null;
                },
                OnPropsEvaluated = (reactUIBuilderData) => reactUIBuilderData.CurrentComponentProp
            };

            var element = builder.Build(new ReactUIBuilderInput{ XmlUI  = xmlUI,DataContext = prop});
            return element;
        }

        static void ShouldRenderSimpleOneDivInnerHTML(Assert assert)
        {
            var xmlUI = "<div width4 ='{Width3}'>" +
                        "   <div>{name}</div>" +
                        "   <div x='{Inner.Name3}'>{Width3}</div>" +
                        "<Component_1 Name5='{Inner.Name3}' />" +
                        "</div>";

            dynamic prop = ObjectLiteral.Create<object>();
            prop.name   = "AbC";
            prop.Width3 = 45;

            dynamic innerObject = ObjectLiteral.Create<object>();
            innerObject.Name3 = "YYç";

            prop.Inner = innerObject;

            var element = BuildUI(xmlUI, prop);

            var container = new jQuery(Document.CreateElement("div"));

            ReactDOM.Render(element, container.Get(0));

            var actual = container.Html();

            var expected = "<div width4=\"45\">" +
                           "<div>AbC</div>" +
                           "<div x=\"YYç\">45</div><a href=\"YYç\"></a>" +
                           "</div>";

            assert.Equal(actual, expected);
        }
        #endregion
    }
}