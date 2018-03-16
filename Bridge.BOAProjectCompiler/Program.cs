namespace Bridge.BOAProjectCompiler
{
    class Program
    {
        #region Methods
        static void Main(string[] args)
        {
            var csprojFile = new CsprojFile
            {
                FileName = "X.csproj"
            };

            csprojFile.WriteToFile( );


            //var bridgeProjectCompiler = new BridgeProjectCompiler
            //{
            //    Input = new BridgeProjectCompilerInput
            //    {
            //        CsprojFilePath = @"D:\github\Bridge.BOAIntegration\Src\Bridge.BOAIntegration.csproj"
            //    }
            //};

            //bridgeProjectCompiler.Compile();
        }
        #endregion
    }
}