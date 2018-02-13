using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using BOA.Common.Types;
using Bridge;
using Bridge.BOAIntegration;
using Bridge.Html5;


namespace BOA.One.Office.CardGeneral.DebitCard
{
    [External]
    public class ExternalResponseCodeContract:ContractBase
    {
        public int externalResponseCode { get; set; }
        public string description { get; set; }
    }

    public class CardTransactionListView: BrowsePage
    {
        public CardTransactionListView(object props) : base(props)
        {
        }



        public GridColumnInfo[] getGridColumns()
        {
            return new[]
            {
                new GridColumnInfo{ Key = "cardNumber", Name = GetMessage("CardGeneral", "CardNumber"), Width = 140, Resizable = true }
            };
        }

        
        

        public void getExternalResponseCodesCommandRespond(GenericResponse<ExternalResponseCodeContract[]> response)
        {
            if (!response.Success)
            {
                return;
            }

            dynamic state = ObjectLiteral.Create<object>();
            state.externalResponseCodeList = response.Value;

            setState(state);



        }

    }

}


namespace BOA.Common.Types
{
    [External]
    public class ContractBase
    {
        [Name("isSelectable")]
        public bool? IsSelectable { get; set; }
        [Name("isSelected")]
        public bool? IsSelected { get; set; }
    }
}

namespace Bridge.BOAIntegration
{
    [External]
    public class Result
    {

    }

    [External]
    public class ResponseBase
    {
        [Name("results")]
        public Result[] Results { get; set; }

        [Name("success")]
        public bool Success { get; set; }
    }

    [External]
    [IgnoreGeneric]
    public class GenericResponse<T>: ResponseBase
    {
        [Name("value")]
        public T Value { get; set; }
    }


    [ObjectLiteral]
    public class GridColumnInfo
    {
        [Name("key")]
        public string Key { get; set; }
        [Name("name")]
        public string Name { get; set; }
        [Name("width")]
        public int Width { get; set; }
        [Name("resizable")]
        public bool Resizable { get; set; }
        

    }

    //[External]
    //[Name("Bridge.$BOAIntegration.BrowsePageInTypeScript")]
    //public class BrowsePageInTypeScript
    //{
    //    [ContractRuntimeIgnored]
    //    public BrowsePageInTypeScript(object props)
    //    {

    //    }
    //}


    public class BrowsePage // : BrowsePageInTypeScript
    {
        public BrowsePage(object props) // :base(props)
        {

            // call base constructor
            Script.Write(@"Bridge.$BOAIntegration.BrowsePageInTypeScript.prototype.constructor.apply(this,[props]);"); 
           

        }
        protected extern void setState(object state);
        public string GetMessage(string groupName, string propertyName)
        {
            return NodeModules.getMessage().Call(null, groupName, propertyName).As<string>();
        }

    }





    [External]
    [Name("React.Component")]
    public abstract class Component
    {
        #region Public Methods
        public abstract ReactElement render();
        #endregion
    }

    [External]
    public class ReactDOM
    {
        #region Public Methods
        [Template("ReactDOM.render({0},{1})")]
        public static extern object Render(ReactElement reactElement, Element container);
        #endregion
    }

    [External]
    public sealed class ReactElement
    {
        #region Public Properties
        public string Key { get; set; }

        public dynamic Props { get; set; }

        public dynamic Ref { get; set; }
        #endregion

        #region Public Methods
        [Template("React.createElement({0},{1},{2})")]
        public static extern ReactElement Create(object reactComponentClass, object props, params object[] children);

        [Template("React.createElement({0},{1})")]
        public static extern ReactElement Create(object reactComponentClass, object props);

        [Template("React.createElement({0})")]
        public static extern ReactElement Create(object reactComponentClass);
        #endregion
    }
}