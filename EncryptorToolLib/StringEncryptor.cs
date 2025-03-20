using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptorToolLib
{
    public class StringEncryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        /// <summary>
        /// 使用指定金鑰初始化加密工具
        /// </summary>
        /// <param name="key">加密金鑰</param>
        public StringEncryptor(string key)
        {
            using (var sha = SHA256.Create())
            {
                // 使用SHA256產生固定長度的key，確保安全性
                _key = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                // 取前16位作為IV
                _iv = new byte[16];
                Array.Copy(_key, 0, _iv, 0, 16);
            }
        }

        /// <summary>
        /// 使用預設金鑰初始化加密工具
        /// </summary>
        public StringEncryptor() : this(EncryptionConstants.GetEncryptionKey())
        {
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                var bytes = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;
                    
                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (var memoryStream = new MemoryStream(bytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}