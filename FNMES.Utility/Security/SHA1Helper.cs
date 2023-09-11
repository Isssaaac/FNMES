using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FNMES.Utility.Security
{
    public static class SHA1Helper
    {
        /// <summary>
        /// 获得一个字符串的加密密文

        /// 此密文为单向加密，即不可逆(解密)密文
        /// </summary>
        /// <param name="plainText">待加密明文</param>
        /// <returns>已加密密文</returns>
        public static string SHA1(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] buffer = sha1.ComputeHash(Encoding.Default.GetBytes(plainText));
            StringBuilder sb = new StringBuilder();
            foreach (byte var in buffer)
            {
                sb.Append(var.ToString("x2"));
            }
            return sb.ToString().ToLower();
        }

        public static string SHA1Lower(string plainText)
        {
            return SHA1(plainText).ToLower();
        }

        public static string SHA1Upper(string plainText)
        {
            return SHA1(plainText).ToUpper();
        }
    }
}
