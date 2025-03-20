using System;

namespace EncryptorToolLib
{
    /// <summary>
    /// 加密工具的常數定義
    /// </summary>
    public static class EncryptionConstants
    {
        /// <summary>
        /// 加密金鑰的環境變數名稱
        /// </summary>
        public const string EncryptionKeyEnvironmentVariable = "ENCRYPTION_KEY";

        /// <summary>
        /// 加密金鑰的預設值
        /// </summary>
        public const string DefaultEncryptionKey = "EncryptorToolDefaultSecureKey2025";

        /// <summary>
        /// 取得加密金鑰 (優先使用環境變數，如果沒有則使用預設值)
        /// </summary>
        /// <returns>加密金鑰</returns>
        public static string GetEncryptionKey()
        {
            return Environment.GetEnvironmentVariable(EncryptionKeyEnvironmentVariable) ?? DefaultEncryptionKey;
        }
    }
}