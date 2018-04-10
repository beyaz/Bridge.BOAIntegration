namespace System.Windows.Data
{
    public class BindingInfoContract
    {
        #region Public Properties
        public BindingMode BindingMode           { get; set; }
        public object      ConverterParameter    { get; set; }
        public string      ConverterTypeFullName { get; set; }
        public string      SourcePath            { get; set; }
        #endregion
    }
}