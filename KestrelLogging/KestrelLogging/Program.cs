using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 設定 Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/kestrel-.log", 
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 1024 * 1024,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// 加入 MVC 服務
builder.Services.AddControllersWithViews();

// 設定 Kestrel 伺服器選項
builder.WebHost.ConfigureKestrel(options =>
{
   // options.ListenAnyIP(5000);
    
    // 啟用詳細的連接日誌
    options.ConfigureEndpointDefaults(endpoint =>
    {
        endpoint.UseConnectionLogging();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
