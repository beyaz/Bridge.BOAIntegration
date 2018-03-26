using System;

namespace Bridge.BOAProjectCompiler
{
    [Serializable]
    public class BOAProjectConfiguration
    {
        #region Public Properties
        public string   AssemblyName { get; set; }
        public string[] SourceFiles  { get; set; }
        public string[] References { get; set; }
        
        #endregion
    }
}