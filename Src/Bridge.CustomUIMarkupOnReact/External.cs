using Bridge.Html5;

namespace Bridge.CustomUIMarkupOnReact
{
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