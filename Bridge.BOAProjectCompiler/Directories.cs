using System;
using System.IO;

namespace Bridge.BOAProjectCompiler
{
    static class Directories
    {
        #region Constants
        public const string BridgeConfigFiles     = SolutionPath + @"Bridge.BOAProjectCompiler\BridgeConfigFiles\";
        public const string DevelopmentFolderName = @"Dev";
        public const string IIS                   = @"D:\BOA\One\wwwroot\";
        public const string SolutionPath          = @"D:\github\Bridge.BOAIntegration\";
        #endregion

        #region Public Properties
        public static string BusinessModules               => @"D:\Work\BOA.BusinessModules\" + DevelopmentFolderName + Path.DirectorySeparatorChar;
        public static string Kernel                        => @"D:\work\BOA.Kernel\" + DevelopmentFolderName + Path.DirectorySeparatorChar;
        public static string Kernel_BOA_Kernel_CardGeneral => Kernel + @"BOA.Kernel.CardGeneral\";

        public static string BOA_Types_Kernel_General=> Kernel + @"BOA.Kernel.Technology\General\BOA.Types.Kernel.General\";



        public static string WorkingDirectory              => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + 
                                                              Path.DirectorySeparatorChar + "Bridge.BOAProjectCompiler" + 
                                                              Path.DirectorySeparatorChar;


        public static string GetDllPath(string assemblyName)
        {
            return WorkingDirectory + $@"{assemblyName}\bin\Debug\{assemblyName}.dll";
        }
        #endregion
    }
}