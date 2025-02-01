using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Logging_kestrel_ap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Logging_kestrel_ap.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
         _logger.LogInformation("訪問首頁");
          return View();
    }

    public IActionResult Privacy()
    {
         _logger.LogInformation("訪問隱私頁面");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("發生錯誤");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
