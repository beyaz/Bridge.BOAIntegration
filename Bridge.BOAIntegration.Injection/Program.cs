using System.Linq;

namespace Bridge.BOAIntegration.Injection
{

    static class Extensions
    {

        /// <summary>
        ///     Removes value from end of str
        /// </summary>
        public static string RemoveFromEnd(this string data, string value)
        {
            if (data.EndsWith(value))
            {
                return data.Substring(0, data.Length - value.Length);
            }

            return data;
        }

    }
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