namespace BOA.Types.CardGeneral.DebitCard
{
    public class Message
    {
        #region Constructors
        public Message()
        {
            ErrorOccurredWhileFetchingData = "Error occurred while fetching data.";
        }
        #endregion

        #region Public Properties
        public string ErrorOccurredWhileFetchingData { get; set; }
        #endregion
    }

    public class Label
    {
        #region Constructors
        public Label()
        {
            CardNumber = Messaging.Helper.GetMessage("CardGeneral", "CardNumber");
        }
        #endregion

        #region Public Properties
        public string CardNumber { get; set; }


        
        #endregion
    }

}