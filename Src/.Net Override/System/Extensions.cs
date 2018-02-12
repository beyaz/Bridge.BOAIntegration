using Bridge;
using Bridge.Html5;

namespace System
{
    static class Extensions
    {
        #region Public Methods
        /// <summary>
        ///     Gets default value of <paramref name="type" />
        /// </summary>
        public static object GetDefaultValue(this Type type)
        {
            if (type.IsClass)
            {
                return null;
            }

            if (type.IsNumeric())
            {
                return Script.Write<object>("Bridge.box(0,type)");
            }

            return Activator.CreateInstance(type);
        }

        public static string GetInnerText(this Element node)
        {
            if (node.NodeType == NodeType.Text)
            {
                return node["textContent"].As<string>();
            }

            return node["innerHTML"].As<string>();
        }

        /// <summary>
        ///     Determines whether this instance is numeric.
        /// </summary>
        public static bool IsNumeric(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(byte) ||
                type == typeof(sbyte) ||
                type == typeof(ushort) ||
                type == typeof(uint) ||
                type == typeof(ulong) ||
                type == typeof(short) ||
                type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(decimal) ||
                type == typeof(double) ||
                type == typeof(float))
            {
                return true;
            }

            return false;
        }

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