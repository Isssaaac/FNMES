using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FNMES.Utility.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class AesHelper
    {
        // 这里将密钥设置为静态常量，您可以根据需要修改
        private const string Key = "FNGZEquiPW#159**"; // 修改为您自己的密钥

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.Mode = CipherMode.ECB; // 设置加密模式为 ECB

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, null); // 不需要 IV


                byte[] encrypted;

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }

                return Convert.ToBase64String(encrypted);
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.Mode = CipherMode.ECB; // 设置加密模式为 ECB


                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, null); // 不需要 IV

                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                string plaintext = null;

                using (var msDecrypt = new System.IO.MemoryStream(cipherTextBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return plaintext;
            }
        }
    }

}
