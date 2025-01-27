namespace CustomRedirectDemo.Middleware;

public static class Custom404MiddlewareExtensions
{
    public static IApplicationBuilder UseCustom404Redirect(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Custom404Middleware>();
    }
}