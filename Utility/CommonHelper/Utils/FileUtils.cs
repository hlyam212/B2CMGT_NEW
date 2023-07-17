using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 針對檔案處理的功能
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// 建立資料夾
        /// </summary>
        /// <param name="directoryPath">資料夾路徑</param>
        /// <exception cref="Exception">directoryPath不得為空</exception>
        public static void CreateDirectoryPath(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new Exception($@"filePath is empty.");
            }

            directoryPath = directoryPath.PathManipulation();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
