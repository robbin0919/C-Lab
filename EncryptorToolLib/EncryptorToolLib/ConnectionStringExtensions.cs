using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EncryptorToolLib
{
    public static class ConnectionStringExtensions
    {
        /// <summary>
        /// 新增解密連線字串的服務
        /// </summary>
        public static IServiceCollection AddConnectionStringDecryption(this IServiceCollection services, IConfiguration configuration)
        {
            string encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? "DefaultEncryptionKey";
            var encryptor = new StringEncryptor(encryptionKey);
            
            // 嘗試解密所有ConnectionStrings__開頭的環境變數
            foreach (var env in Environment.GetEnvironmentVariables().Keys)
            {
                string key = env.ToString();
                if (key.StartsWith("ConnectionStrings__"))
                {
                    string value = Environment.GetEnvironmentVariable(key) ?? string.Empty;
                    if (!string.IsNullOrEmpty(value))
                    {
                        // 嘗試解密
                        string decrypted = encryptor.Decrypt(value);
                        if (decrypted != null)
                        {
                            string configKey = key.Replace("__", ":");
                            // 使用反射來設定連線字串
                            typeof(ConfigurationRoot)
                                .GetMethod("SetConnectionString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                ?.Invoke(configuration, new object[] { key.Substring("ConnectionStrings__".Length), decrypted });
                        }
                    }
                }
            }

            return services;
        }
    }
}