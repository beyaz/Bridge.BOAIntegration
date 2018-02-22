namespace Bridge.BOAIntegration
{
    partial class ReactUIBuilderTest
    {
        #region Methods
        static dynamic BuildUI(string xmlUI, dynamic prop)
        {
            var builder = new ReactUIBuilder
            {
                ComponentClassFinder = tag =>
                {
                    if (tag == nameof(Component_1))
                    {
                        return typeof(Component_1);
                    }

                    return null;
                },
                OnPropsEvaluated = reactUIBuilderData => reactUIBuilderData.CurrentComponentProp
            };

            var element = builder.Build(new ReactUIBuilderInput
            {
                XmlUI       = xmlUI,
                DataContext = prop
            });
            return element;
        }
        #endregion
    }
}