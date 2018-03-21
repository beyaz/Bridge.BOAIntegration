using System;
using System.Globalization;

namespace BOA.Common.Helpers
{
    /// <summary>
    ///     Utility methods for string class.
    /// </summary>
    public static class StringHelper
    {
        #region Public Methods
        /// <summary>
        ///     Determines whether this instance has value.
        /// </summary>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Indicates whether the specified string is null or an System.String.Empty
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        ///     Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Compare strings using ordinal sort rules.
        /// </summary>
        public static bool IsStartsWith(this string data, string value)
        {
            if (data == null)
            {
                return false;
            }

            return data.StartsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Removes the specified value.
        /// </summary>
        public static string Remove(this string data, string value)
        {
            if (data == null)
            {
                return null;
            }

            return data.Replace(value, "");
        }

        /// <summary>
        ///     Removes value from end of str
        /// </summary>
        public static string RemoveFromEnd(this string data, string value)
        {
            if (data.EndsWith(value, StringComparison.CurrentCulture))
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

            if (data.StartsWith(value, StringComparison.CurrentCulture))
            {
                return data.Substring(value.Length, data.Length - value.Length);
            }

            return data;
        }

        /// <summary>
        ///     Returns a copy of this string converted to lowercase, using the casing rules of 'English' culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLowerEN(this string value)
        {
            return value.ToLower(new CultureInfo("en-US"));
        }

        /// <summary>
        ///     Returns a copy of this string converted to lowercase, using the casing rules of 'Turkish' culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLowerTR(this string value)
        {
            return value.ToLower(new CultureInfo("tr-TR"));
        }

        /// <summary>
        ///     Returns a copy of this string converted to uppercase, using the casing rules of 'English' culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUpperEN(this string value)
        {
            return value.ToUpper(new CultureInfo("en-US"));
        }

        /// <summary>
        ///     Returns a copy of this string converted to uppercase, using the casing rules of Turkish culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUpperTR(this string value)
        {
            return value.ToUpper(new CultureInfo("tr-TR"));
        }
        #endregion
    }
}