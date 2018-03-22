using System.IO;

namespace BOA.Common.Helpers
{
    static class FileHelper
    {
        #region Public Methods
        /// <summary>
        ///     Appends to end of file.
        /// </summary>
        public static void AppendToEndOfFile(string filePath, string value)
        {
            var fs = new FileStream(filePath, FileMode.Append);

            var sw = new StreamWriter(fs);
            sw.Write(value);
            sw.Close();
            fs.Close();
        }
        #endregion
    }
}