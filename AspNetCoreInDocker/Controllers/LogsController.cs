using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Globalization;
using System;
public class LogsController : Controller
{
   // private readonly string _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");
    
    private readonly string _logFilePath = "D:\\LAB\\C-Lab\\AspNetCoreInDocker\\bin\\Debug\\net6.0\\logs.txt";
    public IActionResult Index()
    {
         Console.WriteLine("Log added:1");
        var logs = System.IO.File.ReadAllLines(_logFilePath)
          .Select(line => {
        var parts = line.Split(':');
        return new Log
            {
                Timestamp = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                Message = parts[1]
            };
         })
            .ToList();
        return View(logs);
    }

    [HttpPost]
    public IActionResult Create(Log log)
    {
        Console.WriteLine("Log added:2");
        System.IO.File.AppendAllText(_logFilePath, $"{log.Timestamp:yyyy-MM-dd HH:mm:ss}: {log.Message}{Environment.NewLine}");
        Console.WriteLine("Log added:3");
        return RedirectToAction(nameof(Index));
    }
}