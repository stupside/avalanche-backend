using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Avalanche.Application.Http.Middlewares;

public class ValidationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            var json = new
            {
                exception.Errors, exception.Message
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(json);
        }
    }
}