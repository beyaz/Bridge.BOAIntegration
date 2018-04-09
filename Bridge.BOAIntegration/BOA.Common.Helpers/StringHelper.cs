namespace BOA.Common.Helpers
{
    static class StringHelper
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

        /// <summary>
        ///     Removes value from start of str
        /// </summary>
        public static string RemoveFromStart(this string data, string value)
        {
            if (data == null)
            {
                return null;
            }

            if (data.StartsWith(value))
            {
                return data.Substring(value.Length, data.Length - value.Length);
            }

            return data;
        }
        #endregion
    }
}