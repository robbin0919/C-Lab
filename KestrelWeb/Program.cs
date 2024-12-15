
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

 var builder = WebApplication.CreateBuilder(args);
 IPAddress ipAddress = IPAddress.Parse("10.10.10.6");
 builder.WebHost.ConfigureKestrel(serverOptions =>
{
serverOptions.Listen(IPAddress.Any, 5000);
serverOptions.Listen(IPAddress.Loopback, 5001, listenOptions =>
{
listenOptions.UseHttps();
});
});
 var app = builder.Build();

 var html = @"
 <!DOCTYPE html>
 <html>
     <head>
         <meta charset=""utf-8"">
         <title>Log</title>
     </head>
     <body>
         <div>
             <form action=/add method=post target=submitResult>
                 <input placeholder=message name=msg autocomplete=off />
                 <input type=submit value=Add />
             </form>
         </div>
         <iframe name=submitResult style=""display:none""></iframe>
         <iframe src=/logs style=""margin-top:6px""></iframe>
     </body>
 </html>
 ";
 var dataFilePath = Path.Combine(
     app.Environment.ContentRootPath, "logs.txt");
 if (!File.Exists(dataFilePath))
     File.WriteAllText(dataFilePath, "");
 app.MapGet("/", () => Results.Content(html, "text/html"));
 app.MapPost("/add", (HttpRequest request) =>
 {
     var msg = request.Form["msg"];
     File.AppendAllText(dataFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}{Environment.NewLine}");
     return Results.Content(@"<script>
     parent.document.getElementsByTagName('iframe')[1].src=
         '/logs?_=' + new Date().getTime();
     </script>", "text/html");
 });
 app.MapGet("/logs", () => Results.Content(
     File.ReadAllText(dataFilePath), "text/plain"));
 
 app.Run();