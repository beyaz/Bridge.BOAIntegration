using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    public class RequestBase : BOAMessageBase
    {
        #region Public Properties
        [Name("actionId")]
        public short? ActionId { get; set; }

        [Name("customerId")]
        public int? CustomerId { get; set; }

        [Name("languageId")]
        public int? LanguageId { get; set; }

        [Name("mainAccountNumber")]
        public int? MainAccountNumber { get; set; }

        [Name("mainSuffix")]
        public short? MainSuffix { get; set; }

        [Name("methodName")]
        public string MethodName { get; set; }

        [Name("resourceCode")]
        public string ResourceCode { get; set; }

        [Name("resourceId")]
        public int? ResourceId { get; set; }

        [Name("transactionCode")]
        public string TransactionCode { get; set; }

        [Name("windowInstanceId")]
        public object WindowInstanceId { get; set; }
        #endregion
    }
}