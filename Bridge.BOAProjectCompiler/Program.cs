namespace Bridge.BOAProjectCompiler
{
    class Program
    {
        #region Methods
        static void Main()
        {
            Utility.Update_Bridge_BOAIntegration_sourceURL();

            new BOAProjectCompiler().CompileAll();
        }
        #endregion
    }
}