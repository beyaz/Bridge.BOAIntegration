using Bridge;
using Bridge.BOAIntegration;

namespace BOA.One.Office.CardGeneral.DebitCard.CardTransactionList
{
    [ObjectLiteral]
    public class ViewState:BState
    {
        #region Public Properties
        [Name("externalResponseCodeList")]
        public ExternalResponseCodeContract[] ExternalResponseCodeList { get; set; }


        public ComboBoxColumn[] externalResponseCodeListColumns { get; set; }
        #endregion
    }
}