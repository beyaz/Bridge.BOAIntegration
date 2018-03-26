using System;
using System.Collections.Generic;
using System.IO;

namespace Bridge.BOAProjectCompiler
{
    [Serializable]
    public class BOAProjectConfiguration
    {
        #region Public Properties
        public string                AssemblyName { get; set; }
        public IReadOnlyList<string> SourceFiles  { get; set; }
        #endregion
    }

    class BOAProjectCompiler
    {
        public void CompileAll()
        {
            var files = Directory.GetFiles(Directories.ProjectConfigurations,"*.json");

            


        }
    }


}