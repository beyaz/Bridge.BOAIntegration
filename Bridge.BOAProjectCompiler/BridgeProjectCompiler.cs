using System;
using System.IO;
using Bridge.Contract;
using Bridge.Translator;
using Bridge.Translator.Logging;

namespace Bridge.BOAProjectCompiler
{
    class BridgeProjectCompiler
    {
        #region Constants
        public const string BridgeVersion         = "16.8.2";
        const        string OutDir                = @"bin\Debug\";
        const        string PackagesDirectoryPath = Directories.SolutionPath + @"packages\";
        #endregion

        #region Constructors
        public BridgeProjectCompiler()
        {
            BeforeCompile += CopyBridgeConfigJsonFile;
            AfterSuccess  += CopyOutputJsFileToIISFolder;
        }
        #endregion

        #region Public Events
        public event Action AfterSuccess;

        public event Action BeforeCompile;
        #endregion

        #region Public Properties
        public BridgeProjectCompilerData Data { get; set; }
        #endregion

        #region Properties
        string CsProjFileDirectory       => Path.GetDirectoryName(Data.CsprojFilePath) + Path.DirectorySeparatorChar;
        string OutputJsFileDirectoryPath => CsProjFileDirectory + OutDir + "bridge" + Path.DirectorySeparatorChar;
        #endregion

        #region Public Methods
        public void Compile()
        {
            BeforeCompile?.Invoke();

            var bridgeLocation = PackagesDirectoryPath + $@"Bridge.Core.{BridgeVersion}\lib\net40\Bridge.dll";

            var bridgeOptions = new BridgeOptions
            {
                Name = "",
                ProjectProperties = new ProjectProperties
                {
                    AssemblyName = Data.AssemblyName,
                    OutputPath   = OutDir,
                    OutDir       = OutDir
                },
                ProjectLocation = Data.CsprojFilePath,
                OutputLocation  = OutDir,
                DefaultFileName = Data.AssemblyName + ".dll",
                BridgeLocation  = bridgeLocation,
                ExtractCore     = true,
                FromTask        = true
            };

            var logger    = new Logger(null, false, LoggerLevel.Error, true, new ConsoleLoggerWriter(), new FileLoggerWriter());
            var processor = new TranslatorProcessor(bridgeOptions, logger);

            processor.PreProcess();

            processor.Process();
            processor.PostProcess();

            AfterSuccess?.Invoke();
        }
        #endregion

        #region Methods
        void CopyBridgeConfigJsonFile()
        {
            if (IsBOAUIProject())
            {
                File.Copy(Directories.BridgeConfigFiles + "bridge.UIProjects.json", CsProjFileDirectory + "bridge.json", true);
            }
            else
            {
                File.Copy(Directories.BridgeConfigFiles + "bridge.default.json", CsProjFileDirectory + "bridge.json", true);
            }
        }

        void CopyOutputJsFileToIISFolder()
        {
            var assemblyName = Data.AssemblyName;

            // //# sourceURL=BOA.UI.CardGeneral.DebitCard.CampaignTransactionListAssembly.js

            var destFileName = Directories.IIS + assemblyName + ".js";

            File.Copy(OutputJsFileDirectoryPath + assemblyName + ".js", destFileName, true);

            Utility.UpdateSourceURL(destFileName, assemblyName + ".js");

            File.Copy(OutputJsFileDirectoryPath + assemblyName + ".meta.js", @"D:\BOA\One\wwwroot\" + assemblyName + ".meta.js", true);
        }

        bool IsBOAUIProject()
        {
            return Path.GetFileName(Data.CsprojFilePath)?.StartsWith("BOA.UI.") == true;
        }
        #endregion
    }
}