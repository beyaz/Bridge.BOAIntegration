using BOA.Common.Types;

namespace Bridge.BOAIntegration
{
    [ObjectLiteral]
    public class BContext
    {
        #region Public Properties
        [Name("applicationContext")]
        public ApplicationContext ApplicationContext { get; set; }

        [Name("language")]
        public object Language { get; set; }
        #endregion
    }
}