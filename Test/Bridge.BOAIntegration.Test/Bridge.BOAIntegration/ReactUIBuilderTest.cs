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
    }
}