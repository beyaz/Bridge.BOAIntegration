namespace Bridge.BOAIntegration
{
    partial class ReactUIBuilderTest
    {
        #region Methods
       

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