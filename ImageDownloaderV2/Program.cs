using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json; // Add Newtonsoft.Json NuGet package
namespace ImageDownloaderV2;

class ImageDownloader
{
    private readonly string _downloadDirectory= "D:\\LAB\\Images2"; // Or any other default path 
    public ImageDownloader(string configFilePath)
    {
        var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configFilePath));
        _downloadDirectory = config?.DownloadDirectory ?? "D:\\LAB\\Images2";

        
        if (_downloadDirectory != null)
            {
                if (!Directory.Exists(_downloadDirectory))
                {
                    Directory.CreateDirectory(_downloadDirectory);
                }
            }
            else
            {
                // Handle the case where _downloadDirectory is null
                //  - Log an error message
                //  - Throw an exception
                //  - Provide a default directory
            }   
    }

    public async Task DownloadImagesAsync()
    {
        var urls = new List<string>(GetUrlsFromConfiguration());

        var tasks = new List<Task>();
        foreach (var url in urls)
        {
            tasks.Add(DownloadImageAsync(url));
        }

        await Task.WhenAll(tasks);
    }

    private async Task DownloadImageAsync(string url)
    {
        try
        {
            using var webClient = new WebClient();
            var fileName = Path.GetFileName(url);
            var filePath = Path.Combine(_downloadDirectory, fileName);

            await webClient.DownloadFileTaskAsync(url, filePath);
            Console.WriteLine($"Downloaded image: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download image: {url}. Error: {ex.Message}");
        }
    }

    private IEnumerable<string> GetUrlsFromConfiguration()
    {
        var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
        return config.Urls;
    }
}

class Configuration
{
    public IEnumerable<string> Urls { get; set; }
    public string DownloadDirectory { get; set; }
}

class Program
{
    static void Main(string[] args)
  {
        var configFilePath = "config.json"; // Specify the configuration file path

        var imageDownloader = new ImageDownloader(configFilePath);
        imageDownloader.DownloadImagesAsync().Wait();
    }
}
