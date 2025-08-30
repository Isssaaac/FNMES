using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility
{
    public static class PathHelper
    {
        public static string GetDataDirectoryPath()
        {
            var basePath = AppContext.BaseDirectory;
            var dataPath = Path.Combine(basePath, "data");

            // 确保目录存在
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            return dataPath;
        }

        /// <summary>
        /// 获取数据文件的完整路径
        /// </summary>
        public static string GetDataFilePath(string fileName)
        {
            var dataDir = GetDataDirectoryPath();
            return Path.Combine(dataDir, fileName);
        }
    }
}
