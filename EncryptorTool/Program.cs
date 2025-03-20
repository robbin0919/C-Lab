using System;
using System.IO;
using System.Text.Json;

namespace EncryptorTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("連線字串加密工具");
            Console.WriteLine("==================");

            if (args.Length > 0)
            {
                ProcessArguments(args);
                return;
            }

            while (true)
            {
                Console.WriteLine("\n選擇操作:");
                Console.WriteLine("1. 加密連線字串");
                Console.WriteLine("2. 解密連線字串");
                Console.WriteLine("3. 加密appsettings.json中的所有連線字串");
                Console.WriteLine("4. 產生Docker環境變數設定指令");
                Console.WriteLine("0. 離開");

                Console.Write("\n請選擇 (0-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        return;
                    case "1":
                        EncryptString();
                        break;
                    case "2":
                        DecryptString();
                        break;
                    case "3":
                        EncryptAppSettings();
                        break;
                    case "4":
                        GenerateDockerEnvCommands();
                        break;
                    default:
                        Console.WriteLine("無效的選擇，請重試。");
                        break;
                }
            }
        }

        private static void ProcessArguments(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            string command = args[0].ToLower();
            string encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? "DefaultEncryptionKey";

            switch (command)
            {
                case "encrypt":
                    if (args.Length >= 2)
                    {
                        var encryptor = new StringEncryptor(encryptionKey);
                        Console.WriteLine(encryptor.Encrypt(args[1]));
                    }
                    else
                    {
                        PrintUsage();
                    }
                    break;
                case "decrypt":
                    if (args.Length >= 2)
                    {
                        var encryptor = new StringEncryptor(encryptionKey);
                        Console.WriteLine(encryptor.Decrypt(args[1]));
                    }
                    else
                    {
                        PrintUsage();
                    }
                    break;
                case "encryptfile":
                    if (args.Length >= 2)
                    {
                        EncryptAppSettingsFile(args[1], encryptionKey);
                    }
                    else
                    {
                        PrintUsage();
                    }
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("使用方式:");
            Console.WriteLine("  EncryptorTool encrypt <連線字串>       - 加密連線字串");
            Console.WriteLine("  EncryptorTool decrypt <加密連線字串>    - 解密連線字串");
            Console.WriteLine("  EncryptorTool encryptfile <檔案路徑>   - 加密檔案中的所有連線字串");
        }

        private static void EncryptString()
        {
            Console.Write("請輸入加密金鑰 (若為空則使用預設值): ");
            string key = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(key))
                key = "DefaultEncryptionKey";

            Console.Write("請輸入要加密的連線字串: ");
            string connectionString = Console.ReadLine();

            var encryptor = new StringEncryptor(key);
            string encrypted = encryptor.Encrypt(connectionString);

            Console.WriteLine("\n加密結果:");
            Console.WriteLine(encrypted);
            Console.WriteLine("\nDocker環境變數設定:");
            Console.WriteLine($"ENV ConnectionStrings__EncryptedConnection=\"{encrypted}\"");
            Console.WriteLine($"ENV ENCRYPTION_KEY=\"{key}\"");
        }

        private static void DecryptString()
        {
            Console.Write("請輸入加密金鑰 (若為空則使用預設值): ");
            string key = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(key))
                key = "DefaultEncryptionKey";

            Console.Write("請輸入要解密的連線字串: ");
            string encryptedString = Console.ReadLine();

            var encryptor = new StringEncryptor(key);
            string decrypted = encryptor.Decrypt(encryptedString);

            Console.WriteLine("\n解密結果:");
            Console.WriteLine(decrypted ?? "解密失敗，請檢查加密金鑰是否正確");
        }

        private static void EncryptAppSettings()
        {
            Console.Write("請輸入appsettings.json檔案路徑: ");
            string filePath = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("檔案路徑不能為空");
                return;
            }

            Console.Write("請輸入加密金鑰 (若為空則使用預設值): ");
            string key = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(key))
                key = "DefaultEncryptionKey";

            EncryptAppSettingsFile(filePath, key);
        }

        private static void EncryptAppSettingsFile(string filePath, string key)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"找不到檔案: {filePath}");
                    return;
                }

                string json = File.ReadAllText(filePath);
                using JsonDocument doc = JsonDocument.Parse(json);

                var encryptor = new StringEncryptor(key);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings))
                {
                    Console.WriteLine("\n加密後的連線字串:");
                    foreach (JsonProperty property in connectionStrings.EnumerateObject())
                    {
                        string encrypted = encryptor.Encrypt(property.Value.GetString());
                        Console.WriteLine($"{property.Name}: {encrypted}");
                    }

                    Console.WriteLine("\nDocker環境變數設定:");
                    foreach (JsonProperty property in connectionStrings.EnumerateObject())
                    {
                        string encrypted = encryptor.Encrypt(property.Value.GetString());
                        Console.WriteLine($"ENV ConnectionStrings__{property.Name}=\"{encrypted}\"");
                    }
                    Console.WriteLine($"ENV ENCRYPTION_KEY=\"{key}\"");

                    string outputFilePath = Path.Combine(
                        Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath) + ".encrypted.json");

                    // 為了簡單起見，這裡只生成Docker環境變數設定，不修改原檔案
                    Console.WriteLine($"\n環境變數配置已生成。請將這些設定加入到您的Dockerfile或docker-compose.yml中。");
                }
                else
                {
                    Console.WriteLine("在檔案中找不到ConnectionStrings區段");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"處理檔案時發生錯誤: {ex.Message}");
            }
        }

        private static void GenerateDockerEnvCommands()
        {
            Console.Write("請輸入appsettings.json檔案路徑: ");
            string filePath = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("檔案路徑不能為空");
                return;
            }

            Console.Write("請輸入加密金鑰 (若為空則使用預設值): ");
            string key = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(key))
                key = "DefaultEncryptionKey";

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"找不到檔案: {filePath}");
                    return;
                }

                string json = File.ReadAllText(filePath);
                using JsonDocument doc = JsonDocument.Parse(json);

                var encryptor = new StringEncryptor(key);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings))
                {
                    Console.WriteLine("\n# Docker環境變數設定");
                    foreach (JsonProperty property in connectionStrings.EnumerateObject())
                    {
                        string encrypted = encryptor.Encrypt(property.Value.GetString());
                        Console.WriteLine($"ENV ConnectionStrings__{property.Name}=\"{encrypted}\"");
                    }
                    Console.WriteLine($"ENV ENCRYPTION_KEY=\"{key}\"");
                    
                    Console.WriteLine("\n# docker-compose.yml 範例");
                    Console.WriteLine("version: '3'");
                    Console.WriteLine("services:");
                    Console.WriteLine("  api:");
                    Console.WriteLine("    build: .");
                    Console.WriteLine("    environment:");
                    foreach (JsonProperty property in connectionStrings.EnumerateObject())
                    {
                        string encrypted = encryptor.Encrypt(property.Value.GetString());
                        Console.WriteLine($"      - ConnectionStrings__{property.Name}={encrypted}");
                    }
                    Console.WriteLine($"      - ENCRYPTION_KEY={key}");
                }
                else
                {
                    Console.WriteLine("在檔案中找不到ConnectionStrings區段");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"處理檔案時發生錯誤: {ex.Message}");
            }
        }
    }
}
