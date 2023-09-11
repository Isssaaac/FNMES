using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace FNMES.Utility.Security
{
    public static class Base64Helper
    {
        #region Base64加密解密
        /// <summary>
        /// Base64加密，采用指定字符编码方式加密。
        /// </summary>
        /// <param name="input">待加密的明文</param>
        /// <param name="encode">字符编码</param>
        /// <returns></returns>
        public static string Base64Encrypt(this string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }

        /// <summary>
        /// Base64加密，采用UTF8编码方式加密。
        /// </summary>
        /// <param name="input">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encrypt(this string input)
        {
            return Base64Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密，采用UTF8编码方式解密。
        /// </summary>
        /// <param name="input">待解密的秘文</param>
        /// <returns></returns>
        public static string Base64Decrypt(this string input)
        {
            return Base64Decrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密，采用指定字符编码方式解密。
        /// </summary>
        /// <param name="input">待解密的秘文</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Base64Decrypt(this string input, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(input));
        }
        #endregion

        /// <summary>
        /// 文件转base64字符串
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileToBase64(string filePath)
        {
            try
            {
                string base64Str;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bt = new byte[fileStream.Length];
                    // 调用read读取方法  
                    fileStream.Read(bt, 0, bt.Length);
                    base64Str = Convert.ToBase64String(bt);
                }
                return base64Str;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// base64字符串转文件字节数组
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static byte[] FileFromBase64(string base64)
        {
            return Convert.FromBase64String(base64);
        }



        /// <summary>  
        /// 将传进来的文件转换成字符串  
        /// </summary>  
        /// <param name="FilePath">待处理的文件路径(本地或服务器)</param>  
        /// <returns></returns>
        public static string FileToBinary(string filePath)
        {
            //利用新传来的路径实例化一个FileStream对像  
            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //得到对象的大小
            int fileLength = Convert.ToInt32(fs.Length);
            //声明一个byte数组 
            byte[] fileByteArray = new byte[fileLength];
            //声明一个读取二进流的BinaryReader对像
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            for (int i = 0; i < fileLength; i++)
            {
                //将数据读取出来放在数组中 
                br.Read(fileByteArray, 0, fileLength);
            }
            //装数组转换为String字符串
            string strData = Convert.ToBase64String(fileByteArray);
            br.Close();
            fs.Close();
            return strData;
        }

        /// <summary>  
        /// 将传进来的字符串保存为文件  
        /// </summary>  
        /// <param name="path">需要保存的位置路径</param>  
        /// <param name="binary">需要转换的字符串</param>  
        public static void BinaryToFile(string path, string binary)
        {
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //利用新传来的路径实例化一个FileStream对像  
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
            //实例化一个用于写的BinaryWriter  
            byte[] buffer = Convert.FromBase64String(binary);
            bw.Write(buffer);
            bw.Close();
            fs.Close();
        }
    }
}
