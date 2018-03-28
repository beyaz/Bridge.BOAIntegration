using System.Collections.Generic;
using System.IO;

namespace Bridge.BOAProjectCompiler
{
    class CsprojFileCreatorData
    {
        #region Public Properties
        public string AssemblyName { get; set; }

        public string Bridge_BOAIntegration_dll_Path { get; set; } = Directories.SolutionPath + @"Bridge.BOAIntegration\bin\Debug\Bridge.BOAIntegration.dll";
        public string BridgeVersionNumber            => BridgeProjectCompiler.BridgeVersion;
        public string FileName                       { get; set; }
        public string OutputFileDirectory            => WorkingDirectory + AssemblyName + Path.DirectorySeparatorChar;
        public string OutputFilePath                 => OutputFileDirectory + FileName;

        public string PackagesDirectory { get; set; } = Directories.SolutionPath + @"packages\";

        public IReadOnlyList<string> ReferenceAssemblyPaths { get; set; }
        public IReadOnlyList<string> SourceFiles            { get; set; }
        public string                WorkingDirectory       { get; set; } = Directories.WorkingDirectory;
        #endregion
    }
}