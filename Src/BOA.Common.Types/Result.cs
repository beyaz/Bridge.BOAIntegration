using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    public class Result
    {
        #region Public Properties
        [Name("applicationSide")]
        public ApplicationSide ApplicationSide { get; set; }

        [Name("errorCode")]
        public string ErrorCode { get; set; }

        [Name("errorMessage")]
        public string ErrorMessage { get; set; }

        [Name("exception")]
        public string Exception { get; set; }

        [Name("isFriendly")]
        public bool? IsFriendly { get; set; }

        [Name("params")]
        public string[] Params { get; set; }

        [Name("severity")]
        public Severity Severity { get; set; }
        #endregion
    }
}