using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Globalization;
using System;
public class LogsController : Controller
{
   
     // private readonly string _dataFilePath  = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
    
    private readonly string _dataFilePath  = "D:\\LAB\\C-Lab\\AspNetCoreInDocker01\\bin\\Debug\\net6.0\\logs.txt";

    public IActionResult Index()
    {
        var logs = System.IO.File.ReadAllLines(_dataFilePath)
            .Select(line => new Log
            {
                Timestamp = DateTime.ParseExact(line.Split(' ')[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                Message = line.Substring(21)
            })
            .ToList();
            Console.WriteLine("Log added:1");
        return View(logs);
    }

[HttpPost("Create")]
    public IActionResult Create(Log log)
    {
        Console.WriteLine("Log added:2");
        if (!ModelState.IsValid) // 檢查模型驗證
        {
            return View(log);
        }

        try
        {
            // 確保 Message 不為 null
            if (string.IsNullOrEmpty(log.Message))
            {
                ModelState.AddModelError("Message", "Message cannot be empty.");
                return View(log);
            }
            Console.WriteLine("Log added:3");
            // 將日志信息寫入檔案
            System.IO.File.AppendAllText(_dataFilePath, $"{log.Timestamp:yyyy-MM-dd HH:mm:ss} {log.Message}{Environment.NewLine}");

        }
        catch (Exception ex)
        {
            // 處理異常
            Console.Error.WriteLine(ex.Message);
            return StatusCode(500);
        }

        return RedirectToAction("Index");
    }
}