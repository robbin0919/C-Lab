using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ConfigDemo.Models;

namespace ConfigDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public HomeController(
            IOptions<AppSettings> appSettings,
            IConfiguration configuration)
        {
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // 方法1：使用強型別設定
            ViewBag.ApiUrl = _appSettings.ApiUrl;
            ViewBag.MaxItems = _appSettings.MaxItems;
            ViewBag.SmtpServer = _appSettings.EmailSettings.SmtpServer;

            // 方法2：直接從 IConfiguration 讀取
            ViewBag.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            
            // 方法3：使用字串路徑讀取
            ViewBag.SmtpPort = _configuration["AppSettings:EmailSettings:SmtpPort"];

            return View();
        }
    }
}