using Ledgr.Application.Common.Exceptions;
using System.Text.Json;

namespace Ledgr.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            ctx.Response.ContentType = "application/json";

            (ctx.Response.StatusCode, var msg) = ex switch
            {
                UnauthorizedException e => (401, e.Message),
                ForbiddenException    e => (403, e.Message),
                NotFoundException     e => (404, e.Message),
                ValidationException   e => (422, e.Message),
                ConflictException     e => (409, e.Message),
                _                       => (500, "An unexpected error occurred."),
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = msg }));
        }
    }
}
