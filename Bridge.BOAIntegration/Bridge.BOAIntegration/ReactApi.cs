namespace Bridge.BOAIntegration
{
    [External]
    public sealed class ReactElement
    {
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