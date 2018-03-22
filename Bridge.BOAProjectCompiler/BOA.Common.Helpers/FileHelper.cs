using System.IO;
using System.Text;

namespace BOA.Common.Helpers
{
    static class FileHelper
    {
        #region Public Methods
        public static void AppendToEndOfFile(string filePath, string value)
        {
            var fs = new FileStream(filePath, FileMode.Append);

            var sb = new StringBuilder();
            sb.AppendLine(value);
            var sw = new StreamWriter(fs);
            sw.Write(sb);
            sw.Close();
            fs.Close();
        }
        #endregion
    }
}