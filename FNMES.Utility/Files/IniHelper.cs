using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Files
{
    public class IniHelper
    {
        //INI文件名  
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool WriteDB(string key, string value)
        {
            return WriteIniData("db", key, value, AppDomain.CurrentDomain.BaseDirectory + "config.ini");
        }

        public static string ReadDB(string key)
        {
            return ReadIniData("db", key, "", AppDomain.CurrentDomain.BaseDirectory + "config.ini");
        }

        public static bool WriteSystem(string key, string value)
        {
            return WriteIniData("system", key, value, AppDomain.CurrentDomain.BaseDirectory + "config.ini");
        }

        public static string ReadSystem(string key)
        {
            return ReadIniData("system", key, "", AppDomain.CurrentDomain.BaseDirectory + "config.ini");
        }

        #region DeleteSection
        /// <summary>
        /// 删除节
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="fileName">INI文件名</param>
        /// <returns>非零表示成功，零表示失败</returns>
        public static bool DeleteSection(string section, string fileName)
        {
            return WriteIniData(section, null, null, fileName);
        }

        #endregion

        #region DeleteKey
        /// <summary>
        /// 删除键的值
        /// </summary>
        /// <param name="section">节点名</param>
        /// <param name="key">键名</param>
        /// <param name="fileName">INI文件名</param>
        /// <returns>非零表示成功，零表示失败</returns>
        public static bool DeleteKey(string section, string key, string fileName)
        {
            return WriteIniData(section, key, null, fileName);
        }
        #endregion
    }
}
