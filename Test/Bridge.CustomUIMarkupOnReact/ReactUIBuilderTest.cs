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
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(EvaluateAttributeValue_Should_Evaluate_From_BindingPath), EvaluateAttributeValue_Should_Evaluate_From_BindingPath);
            QUnit.QUnit.Test(nameof(BOAIntegration) + "->" + nameof(EvaluateAttributeValue_Should_Return_Function_When_Attribute_Value_Is_Looking_To_Method), EvaluateAttributeValue_Should_Return_Function_When_Attribute_Value_Is_Looking_To_Method);
        }
        #endregion

        #region Methods
        static void EvaluateAttributeValue_Should_Evaluate_From_BindingPath(Assert assert)
        {
            // ARRANGE
            var builder = new ReactUIBuilder();

            // ACT 
            var value = builder.EvaluateAttributeValue("{Model.Name}", JSON.Parse("{\"Model\":{\"Name\":\"t\"}}"));

            // ASSERT
            assert.Equal(value, "t");
        }

        static void EvaluateAttributeValue_Should_Return_Function_When_Attribute_Value_Is_Looking_To_Method(Assert assert)
        {
            // ARRANGE
            var builder = new ReactUIBuilder
            {
                Input = new ReactUIBuilderInput
                {
                    Caller = Script.Write<object>("{A:function(){ }}")
                }
            };

            // ACT 
            var value = builder.EvaluateAttributeValue("this.A", null);

            // ASSERT
            assert.Equal(Script.TypeOf(value), "function");
        }

        static void ShouldRenderSimpleOneDivInnerHTML(Assert assert)
        {
            var xmlUI = "<div width4 ='{Width3}'>" +
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