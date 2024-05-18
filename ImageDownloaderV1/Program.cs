using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
namespace ImageDownloaderV1;
class ImageDownloader
{
    private readonly string _downloadDirectory;

    public ImageDownloader(string downloadDirectory)
    {
        _downloadDirectory = downloadDirectory;
    }

    public async Task DownloadImagesAsync(IEnumerable<string> urls)
    {
        if (!Directory.Exists(_downloadDirectory))
        {
            Directory.CreateDirectory(_downloadDirectory);
        }

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
}
class Program
{
    static void Main(string[] args)
    {
        var urls = new List<string>()
        {
            "https://images2.imgbox.com/66/99/NhAuQujo_o.jpg",
            "https://images2.imgbox.com/83/62/WP86JjLP_o.jpg",
            "https://images2.imgbox.com/1e/18/onDPN72o_o.jpg",
        };

        var downloadDirectory = "D:\\LAB\\Images";

        var imageDownloader = new ImageDownloader(downloadDirectory);
        imageDownloader.DownloadImagesAsync(urls).Wait();
    }
}