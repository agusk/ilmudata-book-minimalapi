using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine("LoggingMiddleware>> Request Incoming");
        await _next(context);
        Console.WriteLine("LoggingMiddleware>> Response Outgoing");
    }
}
