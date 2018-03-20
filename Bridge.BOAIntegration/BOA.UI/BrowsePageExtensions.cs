using System;
using System.Collections.Generic;
using Bridge.BOAIntegration;

namespace BOA.UI
{
    public static class BrowsePageExtensions
    {
        #region Public Methods
        public static void ConfigureColumns(this BrowsePage browseForm, IEnumerable<DataGridColumnInfoContract> columns, bool isSelectionTypeSingle)
        {
            var fields = new object[0];
            foreach (var item in columns)
            {
                var field = new object
                {
                    ["key"] = item.BindingPath,
                    ["name"] = item.Label,
                    ["resizable"] = true
                };

                if (item.DataType?.IsNumeric() == true)
                {
                    field["type"] = "number";
                    field["numberFormat"] = "M";
                }

                fields.Push(field);
            }

            browseForm[AttributeName.DolarColumns] = fields;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Determines whether this instance is numeric.
        /// </summary>
        static bool IsNumeric(this Type type)
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
        #endregion

        static class AttributeName
        {
            #region Constants
            internal const string DolarColumns = "$columns";
            #endregion
        }
    }
}