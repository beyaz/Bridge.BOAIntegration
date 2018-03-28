namespace Bridge.BOAIntegration
{
    [External]
    [Name("React.Component")]
    public abstract class Component
    {
        #region Public Methods
        public abstract ReactElement render();
        #endregion
    }
}