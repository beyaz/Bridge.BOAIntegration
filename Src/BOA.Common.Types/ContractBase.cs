using Bridge;

namespace BOA.Common.Types
{
    [ObjectLiteral]
    public class ContractBase
    {
        [Name("isSelectable")]
        public bool? IsSelectable { get; set; }
        [Name("isSelected")]
        public bool? IsSelected { get; set; }
    }
}