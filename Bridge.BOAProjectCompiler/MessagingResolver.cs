using System.Linq;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    class MessagingResolver
    {
        #region Public Methods
        public static (string GroupName, string PropertyName) GetMessagingExpressionValue(string attributeValue)
        {
            // example: '{m:Messaging Group=CardGeneral, Property=CampaignStatus}'
            var value = attributeValue.Trim().Remove("{m:Messaging ").Trim().RemoveFromStart("Group=").Remove(" Property=").RemoveFromEnd("}");

            var arr = value.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            return (arr[0], arr[1]);
        }

        public static bool IsMessagingExpression(string attributeValue)
        {
            return attributeValue?.Trim().StartsWith("{m:Messaging ") == true;
        }
        #endregion
    }
}