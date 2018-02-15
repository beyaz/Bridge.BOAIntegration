namespace Bridge.BOAIntegration
{
    [ObjectLiteral]
    public class BState
    {
        #region Public Properties
        [Name("context")]
        public BContext Context { get; set; }

        [Name("pageParams")]
        public BPageParams PageParams { get; set; }
        #endregion
    }
}