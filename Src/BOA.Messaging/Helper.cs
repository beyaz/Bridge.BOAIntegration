using System;
using Bridge.BOAIntegration;

namespace BOA.Messaging
{
    public static class Helper
    {
        #region Public Methods
        public static string GetMessage(string groupName, string propertyName)
        {
            return NodeModules.getMessage().Call(null, groupName, propertyName).As<string>();
        }
        #endregion
    }
}