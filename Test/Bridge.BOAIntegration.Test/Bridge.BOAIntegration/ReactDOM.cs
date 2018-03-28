using Bridge.Html5;

namespace Bridge.BOAIntegration
{
    [External]
    public class ReactDOM
    {
        #region Public Methods
        [Template("ReactDOM.render({0},{1})")]
        public static extern object Render(ReactElement reactElement, Element container);
        #endregion
    }
}