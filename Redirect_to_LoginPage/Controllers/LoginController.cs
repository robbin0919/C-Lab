using CustomRedirectDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public class LoginController : Controller
{
    private readonly IOptions<AppSettings> _settings;

    public LoginController(IOptions<AppSettings> settings)
    {
        _settings = settings;
    }

    public IActionResult Index(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
}