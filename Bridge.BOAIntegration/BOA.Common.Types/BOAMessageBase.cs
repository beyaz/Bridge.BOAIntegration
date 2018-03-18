using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    public class BOAMessageBase
    {
        [Name("businessKey")]
        public decimal? BusinessKey { get; set; }
    }
}


