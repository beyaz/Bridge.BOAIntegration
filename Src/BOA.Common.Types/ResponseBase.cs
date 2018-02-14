using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    public class ResponseBase : BOAMessageBase
    {
        #region Public Properties
        [Name("key")]
        public string Key { get; set; }

        [Name("results")]
        public Result[] Results { get; set; }

        [Name("success")]
        public bool Success { get; set; }
        #endregion
    }
}