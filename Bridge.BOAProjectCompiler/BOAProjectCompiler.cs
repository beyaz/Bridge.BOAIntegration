using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Bridge.BOAProjectCompiler
{
    class BOAProjectCompiler
    {
        #region Public Methods
        public void CompileAll()
        {
            foreach (var configuration in GetAllConfigurations())
            {
                var data = new CsprojFileData
                {
                    AssemblyName = configuration.AssemblyName,
                    FileName     = configuration.AssemblyName + ".csproj",
                    SourceFiles  = configuration.SourceFiles
                };

               

                if (configuration.References != null)
                {
                    data.ReferenceAssemblyPaths = configuration.References.ToList().ConvertAll(Directories.GetDllPath);
                }

                var csprojFile = new CsprojFile
                {
                    Data = data

                };

                csprojFile.WriteToFile();

                var bridgeProjectCompiler = new BridgeProjectCompiler
                {
                    Data = new BridgeProjectCompilerData
                    {
                        CsprojFilePath = data.OutputFilePath
                    }
                };

                bridgeProjectCompiler.Compile();
            }
        }
        #endregion

        #region Methods
        static BOAProjectConfiguration NotmalizePaths(BOAProjectConfiguration configuration)
        {
            for (var i = 0; i < configuration.SourceFiles.Length; i++)
            {
                configuration.SourceFiles[i] = configuration.SourceFiles[i]
                                                            .Replace("Kernel->", Directories.Kernel)
                                                            .Replace("BOA->", Directories.BOA)
                                                            .Replace("BusinessModules->", Directories.BusinessModules)
                    ;
            }

            return configuration;
        }

        IReadOnlyList<BOAProjectConfiguration> GetAllConfigurations()
        {
            var files = Directory.GetFiles(Directories.ProjectConfigurations, "*.json");

            var list = new List<BOAProjectConfiguration>();

            foreach (var file in files)
            {
                list.Add(NotmalizePaths(JsonConvert.DeserializeObject<BOAProjectConfiguration>(File.ReadAllText(file))));
            }

            return list;
        }
        #endregion
    }
}