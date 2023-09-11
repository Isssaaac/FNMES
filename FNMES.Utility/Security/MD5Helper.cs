using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FNMES.Utility.Security
{
    public static class MD5Helper
    {
        /// <summary>
        /// 字符串MD5加密。
        /// </summary>
        /// <param name="strOri">需要加密的字符串</param>
        /// <returns></returns>
        public static string md5(this string text)
        {
            return md5(text, Encoding.Default);
        }
        public static string MD5(this string text)
        {
            return MD5(text, Encoding.Default);
        }
        /// <summary>
        /// 字符串MD5加密。
        /// </summary>
        /// <param name="strOri">需要加密的字符串</param>
        /// <returns></returns>
        public static string md5(this string text, Encoding encoder)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(encoder.GetBytes(text));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString().ToLower();
        }
        public static string MD5(this string text, Encoding encoder)
        {
            return md5(text, encoder).ToUpper();
        }
        /// <summary>
        /// 文件流MD5加密。
        /// </summary>
        /// <param name="stream">需要加密的文件流</param>
        /// <returns></returns>
        public static string md5(this Stream stream)
        {
            MD5 md5serv = MD5CryptoServiceProvider.Create();
            byte[] buffer = md5serv.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte var in buffer)
            {
                sb.Append(var.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }
        public static string MD5(this Stream stream)
        {
            return md5(stream).ToUpper();
        }

        #region MD5加密
        /// <summary>
        /// 字符串MD5加密。
        /// </summary>
        /// <param name="strOri">需要加密的字符串</param>
        /// <returns></returns>
        public static string MD5Encrypt(this string text)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(text));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// 文件流MD5加密。
        /// </summary>
        /// <param name="stream">需要加密的文件流</param>
        /// <returns></returns>
        public static string MD5Encrypt(this Stream stream)
        {
            MD5 md5serv = MD5CryptoServiceProvider.Create();
            byte[] buffer = md5serv.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte var in buffer)
            {
                sb.Append(var.ToString("x2"));
            }
            return sb.ToString();
        }
        #endregion
    }
}
