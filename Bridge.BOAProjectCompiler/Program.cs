namespace Bridge.BOAProjectCompiler
{
    class Program
    {
        #region Methods
        static void Main(string[] args)
        {

            Build_BOA_Types_Kernel_DebitCard();
            Build_BOA_Types_CardGeneral_DebitCard();


            //var bridgeProjectCompiler = new BridgeProjectCompiler
            //{
            //    Input = new BridgeProjectCompilerInput
            //    {
            //        CsprojFilePath = @"D:\github\Bridge.BOAIntegration\Src\Bridge.BOAIntegration.csproj"
            //    }
            //};

            //bridgeProjectCompiler.Compile();
        }

        static void Build_BOA_Types_Kernel_DebitCard()
        {
            var tables = Directories.KERNEL_DEV_BOA_Kernel_CardGeneral + @"DebitCard\BOA.Types.Kernel.DebitCard\Tables\";
            var csprojFile = new CsprojFile
            {
                AssemblyName = "BOA.Types.Kernel.DebitCard",
                FileName     = "BOA.Types.Kernel.DebitCard.csproj",
                SourceFiles = new string[]
                {
                    tables+@"ExternalResponseCodeContract.cs",
                    tables+@"ExternalResponseCodeContract.designer.cs",

                    tables+@"DebitTransactionContract.cs",
                    tables+@"DebitTransactionContract.designer.cs",

                    tables+@"DebitTransactionSearchContract.cs",
                    tables+@"DebitTransactionSearchResultContract.cs"


                }
            };

            csprojFile.WriteToFile();


            var bridgeProjectCompiler = new BridgeProjectCompiler
            {
                Input = new BridgeProjectCompilerInput
                {
                    CsprojFilePath = csprojFile.OutputFilePath
                }
            };

            bridgeProjectCompiler.Compile();
        }

        static void Build_BOA_Types_CardGeneral_DebitCard()
        {
          var typesDebitCard= @"D:\work\BOA.BusinessModules\Dev\BOA.CardGeneral.DebitCard\BOA.Types.CardGeneral.DebitCard\";

            var csprojFile = new CsprojFile
            {
                AssemblyName = "BOA.Types.CardGeneral.DebitCard",
                FileName     = "BOA.Types.CardGeneral.DebitCard.csproj",
                SourceFiles = new string[]
                {
                    typesDebitCard+@"Labels.cs",
                    typesDebitCard+@"CardTransaction\CardTransactionRequest.cs",
                    typesDebitCard+@"CardTransaction\CardTransactionRequest.designer.cs"
                },
                ReferenceAssemblyPaths = new []
                {
                    @"D:\Users\beyaztas\Documents\Bridge.BOAProjectCompiler\BOA.Types.Kernel.DebitCard\bin\Debug\BOA.Types.Kernel.DebitCard.dll"
                }
            };

            csprojFile.WriteToFile();


            var bridgeProjectCompiler = new BridgeProjectCompiler
            {
                Input = new BridgeProjectCompilerInput
                {
                    CsprojFilePath = csprojFile.OutputFilePath
                }
            };

            bridgeProjectCompiler.Compile();
        }
        #endregion
    }
}