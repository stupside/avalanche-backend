using Avalanche.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Avalanche.Application.Http.Middlewares;

public class BusinessRuleMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BusinessRuleException exception)
        {
            var json = new
            {
                exception.Message
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(json);
        }
    }
}