using Bridge;

namespace BOA.One.Office.CardGeneral.DebitCard.CardTransactionList
{
    [ObjectLiteral]
    public class ViewState
    {
        #region Public Properties
        [Name("externalResponseCodeList")]
        public ExternalResponseCodeContract[] ExternalResponseCodeList { get; set; }
        #endregion
    }
}