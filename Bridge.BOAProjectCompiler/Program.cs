namespace Bridge.BOAProjectCompiler
{
    class Program
    {
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
                    Directories.WorkingDirectory + @"BOA.Types.Kernel.DebitCard\bin\Debug\BOA.Types.Kernel.DebitCard.dll",
                    Directories.WorkingDirectory + @"BOA.Types.CardGeneral.DebitCard\bin\Debug\BOA.Types.CardGeneral.DebitCard.dll"
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

        #region Methods
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
                    Directories.WorkingDirectory + @"BOA.Types.Kernel.DebitCard\bin\Debug\BOA.Types.Kernel.DebitCard.dll",
                    Directories.WorkingDirectory + @"BOA.Types.CardGeneral.DebitCard\bin\Debug\BOA.Types.CardGeneral.DebitCard.dll"
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
                    typesDebitCard +  "CampaignDefinitionRequest.cs",
                    typesDebitCard +  "CampaignDefinitionRequest.designer.cs"
                },
                ReferenceAssemblyPaths = new[]
                    {Directories.WorkingDirectory + @"BOA.Types.Kernel.DebitCard\bin\Debug\BOA.Types.Kernel.DebitCard.dll"}
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

        static void Build_BOA_Types_Kernel_DebitCard()
        {
            var typesFolder = Directories.Kernel_BOA_Kernel_CardGeneral + @"DebitCard\BOA.Types.Kernel.DebitCard\";
            var tables = typesFolder + @"Tables\";
            var csprojFile = new CsprojFile
            {
                AssemblyName = "BOA.Types.Kernel.DebitCard",
                FileName     = "BOA.Types.Kernel.DebitCard.csproj",
                SourceFiles = new[]
                {
                    tables + @"ExternalResponseCodeContract.cs",
                    tables + @"ExternalResponseCodeContract.designer.cs",

                    tables + @"DebitTransactionContract.cs",
                    tables + @"DebitTransactionContract.designer.cs",

                    tables + @"DebitTransactionSearchContract.cs",
                    tables + @"DebitTransactionSearchResultContract.cs",

                    typesFolder + "DebitCampaignContractMain.cs"
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
            Build_BOA_Types_Kernel_DebitCard();
            Build_BOA_Types_CardGeneral_DebitCard();
            BOA_UI_CardGeneral_DebitCard_CardTransactionListScreen();
            BOA_UI_CardGeneral_DebitCard_CampaignTransactionList();
        }
        #endregion
    }
}