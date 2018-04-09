

namespace System.Windows.Data
{
    public class BindingInfo
    {
      
  

        #region Public Properties
        public BindingMode     BindingMode        { get; set; }
        public IValueConverter Converter          { get; set; }
        public object          ConverterParameter { get; set; }
        public object          Source             { get; set; }
        public PropertyPath    SourcePath         { get; set; }
        public object          Target             { get; set; }
        public PropertyPath    TargetPath         { get; set; }
        #endregion

        #region Public Methods
 

        public void Connect()
        {
            ConnectSourceToTarget();

            if (BindingMode == BindingMode.TwoWay)
            {
                ConnectTargetToSource();
            }
            else
            {
                TargetPath.Walk(Target);
            }

            UpdateTarget();
        }

        public virtual void UpdateSource()
        {
            if (SourcePath.IsNotReadyToUpdate)
            {
                return;
            }

            SourcePath.SetPropertyValue(GetTargetValue());
        }

        public virtual void UpdateTarget()
        {
            if (TargetPath.IsNotReadyToUpdate)
            {
                return;
            }

            var value = SourcePath.GetPropertyValue();

            if (Converter != null)
            {
                value = Converter.Convert(value, null, ConverterParameter, null);
            }

            TargetPath.SetPropertyValue(value);
        }
        #endregion

        #region Methods
        protected virtual void ConnectSourceToTarget()
        {
            SourcePath.Listen(Source, UpdateTarget);
        }

        protected virtual void ConnectTargetToSource()
        {
            TargetPath.Listen(Target, UpdateSource);
        }

        protected virtual object GetTargetValue()
        {
            return TargetPath.GetPropertyValue();
        }



        
        #endregion
    }
}