using BOA.Types.Kernel.DebitCard;
using Bridge;
using Bridge.BOAIntegration;

namespace BOA.One.Office.CardGeneral.DebitCard.CardTransactionListScreen
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