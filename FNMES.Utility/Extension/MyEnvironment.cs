using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Extension
{
    public class MyEnvironment
    {
        private static string _currentPath;
        public static void Init(string currentPath)
        {
            _currentPath = currentPath;
        }

        public static string WebRootPath(string path)
        {
            return RootPath("/wwwroot"+path);
        }

        public static string RootPath(string path)
        {
            string basePath = "";
#if NETFRAMEWORK
            basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("/", "\\");
            if (!basePath.EndsWith("\\"))
            {
                basePath += "\\";
            }
            path = path.Replace("/", "\\");
            if (path.StartsWith("\\"))
            {
                path = path.Substring(1, path.Length - 1);
            }
            return basePath + path;
#else
            if (OperatingSystem.IsWindows())
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("/", "\\");
                if (!basePath.EndsWith("\\"))
                {
                    basePath += "\\";
                }
                path = path.Replace("/", "\\");
                if (path.StartsWith("\\"))
                {
                    path = path.Substring(1, path.Length - 1);
                }
                return basePath + path;
            }
            else
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
                if (!basePath.EndsWith("/"))
                {
                    basePath += "/";
                }
                path = path.Replace("\\", "/");
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1, path.Length - 1);
                }
                return basePath + path;
            }
#endif

        }
    }
}
