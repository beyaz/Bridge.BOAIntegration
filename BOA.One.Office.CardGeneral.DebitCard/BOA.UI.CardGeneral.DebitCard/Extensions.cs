using System;
using System.Collections.Generic;
using System.Reflection;
using BOA.UI.CardGeneral.DebitCard.Common.Data;
using Bridge.BOAIntegration;

namespace BOA.UI.CardGeneral.DebitCard
{


    public static class BrowsePageExtensions
    {
        static class AttributeName
        {
            internal const string DolarColumns = "$columns";
        }



        public static void ConfigureColumns(this BrowsePage browseForm, IEnumerable<DataGridColumnInfoContract> columns)
        {

            var fields = new object[0];
            foreach (var item in columns)
            {
                var field = new object
                {
                    ["key"]       = item.BindingPath,
                    ["name"]      = item.Label,
                    ["resizable"] = true



                };

                if (item.DataType?.IsNumeric() == true)
                {
                    field["type"]         = "number";
                    field["numberFormat"] = "M";
                }

                fields.Push(field);
            }


            browseForm[AttributeName.DolarColumns] = fields;

        }

    }




    /// <summary>
    ///     The extensions
    /// </summary>
    static class Extensions
    {

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


        static BindingFlags AllBindings
        {
            get { return BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic; }
        }

        /// <summary>
        ///     Tries the type of the get proper.
        /// </summary>
        public static Type TryGetProperType(this Type type, string propertyName)
        {
            if (type == null)
            {
                return null;
            }
            var property = type.GetProperty(propertyName, AllBindings);
            if (property == null)
            {
                return null;
            }

            return property.PropertyType;
        }

        #region Methods
        /// <summary>
        ///     Copies the not null values.
        /// </summary>
        internal static void CopyNotNullValues<T>(this T source, T target) where T : class
        {
            if (source == default(T) || target == default(T))
            {
                return;
            }

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                var value = propertyInfo.GetValue(source);
                if (value == null || value.Equals(propertyInfo.PropertyType.GetDefaultValue()))
                {
                    continue;
                }

                propertyInfo.SetValue(target, value);
            }
        }

        /// <summary>
        ///     Gets the default value.
        /// </summary>
        internal static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
        #endregion
    }
}