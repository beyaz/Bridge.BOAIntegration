using System.IO;
using Bridge.Contract;
using Bridge.Translator;

namespace Bridge.BOAProjectCompiler
{
    class BridgeProjectCompiler
    {
        public BridgeProjectCompilerInput Input { get; set; }

        public void Compile()
        {
            var bridgeOptions = new Bridge.Translator.BridgeOptions
            {
                Name = "",
                ProjectProperties = new Bridge.Contract.ProjectProperties
                {
                    AssemblyName = Path.GetFileNameWithoutExtension(Input.CsprojFilePath),
                    OutputPath   = @"bin\Debug\",
                    OutDir       = @"bin\Debug\",
                },
                ProjectLocation = Input.CsprojFilePath,
                OutputLocation  = @"bin\Debug\",
                DefaultFileName = Path.GetFileNameWithoutExtension(Input.CsprojFilePath) + ".dll",
                BridgeLocation  = @"D:\github\Bridge.BOAIntegration\packages\Bridge.Core.16.7.1\lib\net40\Bridge.dll",
                ExtractCore     = true,
                FromTask        = true,
            };

            var logger    = new Bridge.Translator.Logging.Logger(null, false, LoggerLevel.Info, true, new Bridge.Translator.Logging.ConsoleLoggerWriter(), new Bridge.Translator.Logging.FileLoggerWriter());
            var processor = new TranslatorProcessor(bridgeOptions, logger);

            processor.PreProcess();

            processor.Process();
            processor.PostProcess();
        }
    }
}