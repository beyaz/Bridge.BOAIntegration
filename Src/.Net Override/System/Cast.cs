using System.Globalization;

namespace System
{
    /// <summary>
    ///     Utility methods for casting operations
    /// </summary>
    public static class Cast
    {
        #region Public Methods
        /// <summary>
        ///     To the specified value.
        /// </summary>
        public static object To(object value, Type targetType, IFormatProvider provider)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (value == null)
            {
                return targetType.GetDefaultValue();
            }

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var valueAsString = value as string;
                if (valueAsString == string.Empty)
                {
                    return null;
                }

                return To(value, targetType.GetGenericArguments()[0], provider);
            }

            var valueType = value.GetType();

            if (valueType == targetType || targetType == typeof(object) || targetType.IsInstanceOfType(value))
            {
                return value;
            }

            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(value, provider);
            }

            if (targetType == typeof(char))
            {
                return Convert.ToChar(value, provider);
            }

            if (targetType == typeof(sbyte))
            {
                return Convert.ToSByte(value, provider);
            }

            if (targetType == typeof(byte))
            {
                return Convert.ToByte(value, provider);
            }

            if (targetType == typeof(short))
            {
                return Convert.ToInt16(value, provider);
            }

            if (targetType == typeof(ushort))
            {
                return Convert.ToUInt16(value, provider);
            }

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(value, provider);
            }

            if (targetType == typeof(uint))
            {
                return Convert.ToUInt32(value, provider);
            }

            if (targetType == typeof(long))
            {
                return Convert.ToInt64(value, provider);
            }

            if (targetType == typeof(ulong))
            {
                return Convert.ToUInt64(value, provider);
            }

            if (targetType == typeof(float))
            {
                return Convert.ToSingle(value, provider);
            }

            if (targetType == typeof(double))
            {
                return Convert.ToDouble(value, provider);
            }

            if (targetType == typeof(decimal))
            {
                return Convert.ToDecimal(value, provider);
            }

            if (targetType == typeof(DateTime))
            {
                return Convert.ToDateTime(value, provider);
            }

            if (targetType == typeof(string))
            {
                return Convert.ToString(value, provider);
            }

            throw new InvalidCastException("@value:" + value + "not convertible to " + targetType.FullName);
        }

        /// <summary>
        ///     Casts value to 'TTargetType'
        /// </summary>
        public static TTargetType To<TTargetType>(object value, IFormatProvider provider)
        {
            return (TTargetType)To(value, typeof(TTargetType), provider);
        }

        /// <summary>
        ///     Casts value to 'TTargetType'
        /// </summary>
        public static TTargetType To<TTargetType>(object value)
        {
            return To<TTargetType>(value, CultureInfo.InvariantCulture);
        }
        #endregion
    }
}