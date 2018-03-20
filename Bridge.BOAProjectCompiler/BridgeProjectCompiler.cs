using System.IO;
using Bridge.Contract;
using Bridge.Translator;
using Bridge.Translator.Logging;

namespace Bridge.BOAProjectCompiler
{
    class BridgeProjectCompiler
    {
        #region Constants
        const string BridgeVersion         = "16.8.2";
        const string OutDir                = @"bin\Debug\";
        const string PackagesDirectoryPath = @"D:\github\Bridge.BOAIntegration\packages\";
        #endregion

        #region Public Properties
        public BridgeProjectCompilerInput Input { get; set; }
        #endregion

        #region Properties
        string OutputJsFileDirectoryPath => Path.GetDirectoryName(Input.CsprojFilePath) + Path.DirectorySeparatorChar + OutDir + "bridge" + Path.DirectorySeparatorChar;
        #endregion

        #region Public Methods
        public void Compile()
        {
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

            File.Copy(OutputJsFileDirectoryPath + assemblyName + ".js", @"D:\BOA\One\wwwroot\" + assemblyName + ".js", true);
            File.Copy(OutputJsFileDirectoryPath + assemblyName + ".meta.js", @"D:\BOA\One\wwwroot\" + assemblyName + ".meta.js", true);
        }
        #endregion
    }
}