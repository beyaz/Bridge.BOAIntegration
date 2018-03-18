using System;
using System.ComponentModel;

namespace BOA.Common.Helpers
{
    /// <summary>
    ///     Utility methods for INotifyPropertyChanged interface
    /// </summary>
    public static class INotifyPropertyChangedHelper
    {
        #region Public Methods
        /// <summary>
        ///     invoke action when propertyName raised
        /// </summary>
        public static void OnPropertyChanged(this INotifyPropertyChanged notifyPropertyChanged, string propertyName, Action action)
        {
            if (notifyPropertyChanged == null)
            {
                throw new ArgumentNullException("notifyPropertyChanged");
            }

            notifyPropertyChanged.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    action();
                }
            };
        }
        #endregion
    }
}