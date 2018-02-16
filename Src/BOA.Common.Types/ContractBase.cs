using System;
using System.ComponentModel;

namespace BOA.Common.Types
{
    [Serializable]
    public abstract class ContractBase : INotifyPropertyChanged
    {
        #region Public Members
        bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value == isSelected)
                {
                    return;
                }

                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        bool isSelectable = true;

        public bool IsSelectable
        {
            get { return isSelectable; }
            set
            {
                if (value == isSelectable)
                {
                    return;
                }

                isSelectable = value;
                OnPropertyChanged("IsSelectable");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}