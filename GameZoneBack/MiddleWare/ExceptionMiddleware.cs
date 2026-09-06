namespace GameZoneBack.MiddleWare;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
        }
    }
}
