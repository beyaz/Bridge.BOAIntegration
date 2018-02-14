using System.Linq;

namespace Bridge.BOAIntegration.Injection
{
    class Program
    {
        #region Methods
        static void Main(string[] args)
        {
            // args = new[] {"cardgeneral.debitcard.card-transaction-list.js,BOA.One.Office.CardGeneral.DebitCard.CardTransactionList.View"};

            args = args[0].Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var jsFileName   = args[0];
            var typeFullName = args[1];

            new Injector().Inject(new InjectInfo
            {
                SourceJsFilePath = @"D:\BOA\One\wwwroot\" + jsFileName,

                ViewTypeFullName = typeFullName
            });
        }
        #endregion
    }
}