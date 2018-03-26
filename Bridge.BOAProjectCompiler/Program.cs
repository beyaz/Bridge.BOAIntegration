namespace Bridge.BOAProjectCompiler
{
    class Program
    {
        #region Methods
        static void BOA_UI_CardGeneral_DebitCard_CampaignTransactionList()
        {
            var sourceProjectLocation = Directories.BusinessModules + @"BOA.CardGeneral.DebitCard\UI\BOA.UI.CardGeneral.DebitCard.CampaignTransactionList\";

            var csprojFile = new CsprojFile
            {
                AssemblyName = "BOA.UI.CardGeneral.DebitCard.CampaignTransactionListAssembly",
                FileName     = "BOA.UI.CardGeneral.DebitCard.CampaignTransactionListAssembly.csproj",
                SourceFiles = new[]
                {
                    sourceProjectLocation + @"CampaignTransactionList.xaml.cs",
                    sourceProjectLocation + @"CampaignTransactionList.xaml"
                },
                ReferenceAssemblyPaths = new[]
                {
                    Directories.GetDllPath("BOA.Types.Kernel.DebitCard"),
                    Directories.GetDllPath("BOA.Types.CardGeneral.DebitCard")
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

        static void BOA_UI_CardGeneral_DebitCard_CardTransactionListScreen()
        {
            var sourceProjectLocation = Directories.BusinessModules + @"BOA.CardGeneral.DebitCard\UI\BOA.UI.CardGeneral.DebitCard.CardTransactionList\CardTransactionListScreen\";

            var csprojFile = new CsprojFile
            {
                AssemblyName = "BOA.UI.CardGeneral.DebitCard.CardTransactionList",
                FileName     = "BOA.UI.CardGeneral.DebitCard.CardTransactionList.csproj",
                SourceFiles = new[]
                {
                    sourceProjectLocation + @"Extensions.cs",
                    sourceProjectLocation + @"Model.cs",
                    sourceProjectLocation + @"View.xaml.cs",
                    sourceProjectLocation + @"View.xaml"
                },
                ReferenceAssemblyPaths = new[]
                {
                    Directories.GetDllPath("BOA.Types.Kernel.DebitCard"),
                    Directories.GetDllPath("BOA.Types.CardGeneral.DebitCard")
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
            var typesDebitCard = Directories.BusinessModules + @"BOA.CardGeneral.DebitCard\BOA.Types.CardGeneral.DebitCard\";

            var csprojFile = new CsprojFile
            {
                AssemblyName = "BOA.Types.CardGeneral.DebitCard",
                FileName     = "BOA.Types.CardGeneral.DebitCard.csproj",
                SourceFiles = new[]
                {
                    typesDebitCard + @"Labels.cs",
                    typesDebitCard + @"CardTransaction\CardTransactionRequest.cs",
                    typesDebitCard + @"CardTransaction\CardTransactionRequest.designer.cs",
                    typesDebitCard + "CampaignDefinitionRequest.cs",
                    typesDebitCard + "CampaignDefinitionRequest.designer.cs"
                },
                ReferenceAssemblyPaths = new[]
                {
                    Directories.GetDllPath("BOA.Types.Kernel.DebitCard")
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

        static void Main()
        {
            Utility.Update_Bridge_BOAIntegration_sourceURL();


            new BOAProjectCompiler().CompileAll();

          
            Build_BOA_Types_CardGeneral_DebitCard();
            BOA_UI_CardGeneral_DebitCard_CardTransactionListScreen();
            BOA_UI_CardGeneral_DebitCard_CampaignTransactionList();
        }
        #endregion
    }
}