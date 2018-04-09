using System;
using Bridge.Html5;
using Bridge.jQuery2;
using Bridge.QUnit;

namespace Bridge.BOAIntegration
{
    partial class ReactUIBuilderTest
    {
        #region Public Methods
        public static void Register()
        {
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(ShouldRenderSimpleOneDivInnerHTML), ShouldRenderSimpleOneDivInnerHTML);
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(EvaluateAttributeValue.Should_Evaluate_From_BindingPath), EvaluateAttributeValue.Should_Evaluate_From_BindingPath);
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(EvaluateAttributeValue.Should_Return_Function_When_Attribute_Value_Is_Looking_To_Method), EvaluateAttributeValue.Should_Return_Function_When_Attribute_Value_Is_Looking_To_Method);
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(BuildNodeAsText.Should_Evaluate_From_Binding_Path), BuildNodeAsText.Should_Evaluate_From_Binding_Path);
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(BuildNodeAsText.Should_Return_Null_When_InnerText_Is_Empty), BuildNodeAsText.Should_Return_Null_When_InnerText_Is_Empty);
        }
        #endregion

        #region Methods
        static void ShouldRenderSimpleOneDivInnerHTML(Assert assert)
        {

            var clickCount = 0;
            var caller = ObjectLiteral.Create<object>();
            caller["HandleClick"] = (Action) (() => { clickCount++;});


            var xml = "<div width4 ='{Width3}'>" +
                        "   <div id ='x5' onClick='HandleClick'>{name}</div>" +
                        "   <div x='{Inner.Name3}'>{Width3}</div>" +
                        "<Component_1 Name5='{Inner.Name3}' />" +
                        "</div>";

            var prop = Script.Write<object>(@"
{
    name:'AbC', 
    Width3:45,
    Inner: 
    {
        Name3:'YYç'
    }

}");




            #region auto generated

            
            object attributes = null;
            var builder = new UIBuilder
            {
                Caller = caller,
                ComponentClassFinder = ComponentClassFinderMethod,
                DataContext = prop
            };

            attributes = ObjectLiteral.Create<object>();
            attributes["width4"] = new System.Windows.Data.BindingInfoContract
            {
                BindingMode = System.Windows.Data.BindingMode.TwoWay,
                SourcePath  = "Width3"
            };
            builder.Create("div", attributes);

            attributes            = ObjectLiteral.Create<object>();
            attributes["id"] = "x5";
            attributes["onClick"] = caller["HandleClick"];
            attributes["innerHTML"] = new System.Windows.Data.BindingInfoContract
            {
                BindingMode = System.Windows.Data.BindingMode.TwoWay,
                SourcePath  = "name"
            };
            builder.Create("div", attributes);
            builder.EndOf();


            attributes = ObjectLiteral.Create<object>();
            attributes["x"] = new System.Windows.Data.BindingInfoContract
            {
                BindingMode = System.Windows.Data.BindingMode.TwoWay,
                SourcePath  = "Inner.Name3"
            };
            attributes["innerHTML"] = new System.Windows.Data.BindingInfoContract
            {
                BindingMode = System.Windows.Data.BindingMode.TwoWay,
                SourcePath  = "Width3"
            };
            builder.Create("div", attributes);
            builder.EndOf();


            attributes = ObjectLiteral.Create<object>();
            attributes["Name5"] = new System.Windows.Data.BindingInfoContract
            {
                BindingMode = System.Windows.Data.BindingMode.TwoWay,
                SourcePath  = "Inner.Name3"
            };
            builder.Create("Component_1", attributes);
            builder.EndOf();

            builder.EndOf();

            #endregion









            var element = builder.Result.As<ReactElement>();

            var container = new jQuery(Document.CreateElement("div"));

            ReactDOM.Render(element, container.Get(0));

            var actual = container.Html();


            var expected = "<div width4=\"45\">" +
                           "<div id=\"x5\">AbC</div>" +
                           "<div x=\"YYç\">45</div><a href=\"YYç\"></a>" +
                           "</div>";

            assert.Equal(actual, expected);



            container.AppendTo(Document.Body);
            assert.Async();
            jQuery.Ready(() =>
            {
                
                container.Find("#x5").Click();

                assert.Equal(clickCount, 1);

                container.Remove();


            });


        }
        #endregion

        class BuildNodeAsText
        {
            #region Methods
            internal static void Should_Evaluate_From_Binding_Path(Assert assert)
            {
                // ARRANGE
                var dataContext = JSON.Parse("{\"Model\":{\"Name\":\"t\"}}");

                // ACT 
                var value = ReactUIBuilder.BuildNodeAsText("{Model.Name}", dataContext);

                // ASSERT
                assert.Equal(value, "t");
            }

            internal static void Should_Return_Null_When_InnerText_Is_Empty(Assert assert)
            {
                // ARRANGE
                var dataContext = JSON.Parse("{\"Model\":{\"Name\":\"t\"}}");

                // ACT 
                var value = ReactUIBuilder.BuildNodeAsText("{Model.Name}", dataContext);

                // ASSERT
                assert.Equal(value, "t");
            }
            #endregion
        }

        class EvaluateAttributeValue
        {
            #region Methods
            internal static void Should_Evaluate_From_BindingPath(Assert assert)
            {
                // ARRANGE
                var builder     = new ReactUIBuilder();
                var dataContext = JSON.Parse("{\"Model\":{\"Name\":\"t\"}}");

                // ACT 
                var value = builder.EvaluateAttributeValue("{Model.Name}", dataContext);

                // ASSERT
                assert.Equal(value, "t");
            }

            internal static void Should_Return_Function_When_Attribute_Value_Is_Looking_To_Method(Assert assert)
            {
                // ARRANGE
                var builder = new ReactUIBuilder
                {
                    Caller = Script.Write<object>("{A:function(){ }}")
                };

                // ACT 
                var value = builder.EvaluateAttributeValue("this.A", null);

                // ASSERT
                assert.Equal(Script.TypeOf(value), "function");
            }
            #endregion
        }
    }
}