namespace Bridge.BOAIntegration.Injection
{
    static class Extensions
    {
        #region Public Methods
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
        #endregion
    }
}