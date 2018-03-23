using System.IO;
using BOA.Common.Helpers;

namespace Bridge.BOAProjectCompiler
{
    static class Utility
    {
        #region Static Fields
        static bool IsAlreadyUpdated_Update_Bridge_BOAIntegration_sourceURL;
        #endregion

        #region Public Methods
        public static void Update_Bridge_BOAIntegration_sourceURL()
        {
            if (IsAlreadyUpdated_Update_Bridge_BOAIntegration_sourceURL)
            {
                return;
            }

            var path = Directories.IIS + "Bridge.BOAIntegration.js";

            if (File.Exists(path) == false)
            {
                return;
            }

            var jsCode             = File.ReadAllText(path);
            var sourceURLIsWritten = jsCode.Trim().EndsWith("//# sourceURL=Bridge.BOAIntegration.js");
            if (sourceURLIsWritten)
            {
                return;
            }

            UpdateSourceURL(path, "Bridge.BOAIntegration.js");

            IsAlreadyUpdated_Update_Bridge_BOAIntegration_sourceURL = true;
        }

        public static void UpdateSourceURL(string destFileName, string jsFileUrl)
        {
            FileHelper.AppendToEndOfFile(destFileName, "//# sourceURL=" + jsFileUrl);
        }
        #endregion
    }
}