using Avalanche.Application.Http.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Avalanche.Application.Http;

public static class WebApplicationExtension
{
    public static void UseAvalancheApi(this WebApplication application)
    {
        application.MapControllers();

        application.UseSwagger();
        application.UseSwaggerUI();

        application.UseMiddleware<ValidationMiddleware>();

        application.UseMiddleware<BusinessRuleMiddleware>();
        application.UseMiddleware<NotFoundMiddleware>();
    }
}