using System;
using System.IO;

namespace Bridge.BOAProjectCompiler
{
    static class Directories
    {
        #region Constants
        public const string KERNEL_DEV                        = @"D:\work\BOA.Kernel\Dev\";
        public const string KERNEL_DEV_BOA_Kernel_CardGeneral = KERNEL_DEV + @"BOA.Kernel.CardGeneral\";
        #endregion

        #region Public Properties
        public static string WorkingDirectory => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "Bridge.BOAProjectCompiler" + Path.DirectorySeparatorChar;
        #endregion
    }
}