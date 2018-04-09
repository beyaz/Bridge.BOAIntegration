namespace Bridge.BOAIntegration
{
    partial class ReactUIBuilderTest
    {
        #region Methods
        static dynamic BuildUI(string xmlUI, dynamic prop)
        {
        

            var builder = new ReactUIBuilder
            {
                ComponentClassFinder = ComponentClassFinderMethod,

                XmlUI       = xmlUI,
                DataContext = prop
            };

            var element = builder.Build();
            return element;
        }

        static object ComponentClassFinderMethod(string tag)
        {
           
                if (tag == nameof(Component_1))
                {
                    return typeof(Component_1);
                }

                return null;
            
        }
        #endregion
    }
}