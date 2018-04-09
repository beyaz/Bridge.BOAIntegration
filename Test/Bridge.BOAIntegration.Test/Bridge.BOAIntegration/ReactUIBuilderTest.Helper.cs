namespace Bridge.BOAIntegration
{
    partial class ReactUIBuilderTest
    {
        #region Methods
        static dynamic BuildUI(string xmlUI, dynamic prop)
        {
            var componentClassFinder = ComponentClassFinderMethod();

            var builder = new ReactUIBuilder
            {
                ComponentClassFinder = componentClassFinder,

                XmlUI       = xmlUI,
                DataContext = prop
            };

            var element = builder.Build();
            return element;
        }

        static ComponentClassFinder ComponentClassFinderMethod()
        {
            return tag =>
            {
                if (tag == nameof(Component_1))
                {
                    return typeof(Component_1);
                }

                return null;
            };
        }
        #endregion
    }
}