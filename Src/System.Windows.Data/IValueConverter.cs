using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.Windows.Data
{
    public interface IValueConverter
    {
        #region Public Methods
        object Convert(object     value, Type targetType, object parameter, CultureInfo culture);
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
        #endregion
    }
    
}