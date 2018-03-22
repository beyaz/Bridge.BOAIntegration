using System.IO;
using Bridge.Contract;
using Bridge.Translator;
using Bridge.Translator.Logging;
using FileHelper = BOA.Common.Helpers.FileHelper;

namespace Bridge.BOAProjectCompiler
{
    class BridgeProjectCompiler
    {
        #region Constants
        const string BridgeVersion         = "16.8.2";
        const string OutDir                = @"bin\Debug\";
        const string PackagesDirectoryPath = Directories.SolutionPath + @"packages\";
        #endregion

        #region Public Properties
        public BridgeProjectCompilerInput Input { get; set; }
        #endregion

        #region Properties
        string CsProjFileDirectory       => Path.GetDirectoryName(Input.CsprojFilePath) + Path.DirectorySeparatorChar;
        string OutputJsFileDirectoryPath => CsProjFileDirectory + OutDir + "bridge" + Path.DirectorySeparatorChar;
        #endregion

        #region Public Methods
        public void Compile()
        {
            CopyBridgeConfigJsonFile();

            var bridgeLocation = PackagesDirectoryPath + $@"Bridge.Core.{BridgeVersion}\lib\net40\Bridge.dll";

            var bridgeOptions = new BridgeOptions
            {
                Name = "",
                ProjectProperties = new ProjectProperties
                {
                    AssemblyName = Path.GetFileNameWithoutExtension(Input.CsprojFilePath),
                    OutputPath   = OutDir,
                    OutDir       = OutDir
                },
                ProjectLocation = Input.CsprojFilePath,
                OutputLocation  = OutDir,
                DefaultFileName = Path.GetFileNameWithoutExtension(Input.CsprojFilePath) + ".dll",
                BridgeLocation  = bridgeLocation,
                ExtractCore     = true,
                FromTask        = true
            };

            var logger    = new Logger(null, false, LoggerLevel.Info, true, new ConsoleLoggerWriter(), new FileLoggerWriter());
            var processor = new TranslatorProcessor(bridgeOptions, logger);

            processor.PreProcess();

            processor.Process();
            processor.PostProcess();

            var assemblyName = bridgeOptions.ProjectProperties.AssemblyName;

            // //# sourceURL=BOA.UI.CardGeneral.DebitCard.CampaignTransactionListAssembly.js

            var destFileName = @"D:\BOA\One\wwwroot\" + assemblyName + ".js";

            File.Copy(OutputJsFileDirectoryPath + assemblyName + ".js", destFileName, true);
            FileHelper.AppendToEndOfFile(destFileName, "//# sourceURL=" + assemblyName + ".js");

            File.Copy(OutputJsFileDirectoryPath + assemblyName + ".meta.js", @"D:\BOA\One\wwwroot\" + assemblyName + ".meta.js", true);
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

        bool IsBOAUIProject()
        {
            return Input.CsprojFilePath.StartsWith("BOA.UI.");
        }
        #endregion
    }
}