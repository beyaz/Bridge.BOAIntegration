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
            var xml = "<div width4 ='{Width3}'>" +
                        "   <div>{name}</div>" +
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

            var element = BuildUI(xml, prop);

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