using FNMES.Utility.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FNMES.Utility.Security
{
    public static class DESHelper
    {
        #region DES加密解密

        /// <summary>
        /// 默认密钥。
        /// </summary>
        private const string DESENCRYPT_KEY = "hsdjxlzf";

        /// <summary>
        /// DES加密，使用自定义密钥。
        /// </summary>
        /// <param name="text">待加密的明文</param>
        /// <param name="key">8位字符的密钥字符串</param>
        /// <returns></returns>
        public static string DESEncrypt(this string text, string key)
        {
            if (key.Length != 8)
            {
                key = DESENCRYPT_KEY;
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(text);

            byte[] a = ASCIIEncoding.ASCII.GetBytes(key);
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);//将第一个参数转换为十六进制数,长度为2,不足前面补0
            }
            return ret.ToString();
        }

        /// <summary>
        /// DES解密，使用自定义密钥。
        /// </summary>
        /// <param name="cyphertext">待解密的秘文</param>
        /// <param name="key">必须是8位字符的密钥字符串(不能有特殊字符)</param>
        /// <returns></returns>
        public static string DESDecrypt(this string cyphertext, string key)
        {
            if (key.Length != 8)
            {
                key = DESENCRYPT_KEY;
            }
            if (string.IsNullOrEmpty(cyphertext))
                return string.Empty;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[cyphertext.Length / 2];
            for (int x = 0; x < cyphertext.Length / 2; x++)
            {
                int i = (Convert.ToInt32(cyphertext.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
        }

        /// <summary>
        /// DES加密，使用默认密钥。
        /// </summary>
        /// <param name="text">待加密的明文</param>
        /// <returns></returns>
        public static string DESEncrypt(this string text)
        {
            return DESEncrypt(text, DESENCRYPT_KEY);
        }

        /// <summary>
        /// DES解密，使用默认密钥。
        /// </summary>
        /// <param name="cyphertext">待解密的秘文</param>
        /// <returns></returns>
        public static string DESDecrypt(this string cyphertext)
        {
            return DESDecrypt(cyphertext, DESENCRYPT_KEY);
        }
        #endregion
    }
}
