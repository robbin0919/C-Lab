using Microsoft.Extensions.Options;
using CustomRedirectDemo.Models;

namespace CustomRedirectDemo.Middleware;

public class Custom404Middleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<AppSettings> _settings;
    private readonly ILogger<Custom404Middleware> _logger;

    public Custom404Middleware(
        RequestDelegate next, 
        IOptions<AppSettings> settings,
        ILogger<Custom404Middleware> logger)
    {
        _next = next;
        _settings = settings;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == 404)
        {
            _logger.LogWarning($"404 Error for URL: {context.Request.Path}");
            
            var loginPage = _settings.Value.LoginPage;
            var returnUrl = context.Request.Path.Value;
            
            if (!context.Request.Path.StartsWithSegments(loginPage))
            {
                context.Response.Redirect(
                    $"{loginPage}?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
        }
    }
}