using System.IO;

namespace Bridge.BOAProjectCompiler
{
    public class BridgeProjectCompilerData
    {
        #region Public Properties
        public string AssemblyName   => Path.GetFileNameWithoutExtension(CsprojFilePath);
        public string CsprojFilePath { get; set; }
        #endregion
    }
}