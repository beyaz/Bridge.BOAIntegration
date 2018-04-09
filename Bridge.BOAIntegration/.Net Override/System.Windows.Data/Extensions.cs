namespace System.Windows.Data
{
    static class Extensions
    {
        #region Public Methods
        public static BindingInfo ToBindingInfo(this BindingInfoContract contract)
        {
            if (contract == null)
            {
                return null;
            }

            IValueConverter valueConverter = null;
            if (contract.ConverterTypeFullName != null)
            {
                var converterType = Type.GetType(contract.ConverterTypeFullName);
                if (converterType == null)
                {
                    throw new MissingMemberException(contract.ConverterTypeFullName);
                }

                valueConverter = (IValueConverter) Activator.CreateInstance(converterType);
            }

            return new BindingInfo
            {
                SourcePath         = contract.SourcePath,
                BindingMode        = contract.BindingMode,
                Converter          = valueConverter,
                ConverterParameter = contract.ConverterParameter
            };
        }
        #endregion
    }
}