using Avalanche.Application.Http.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Application.Http;

public static class WebApplicationBuilderExtension
{
    public static void UseAvalancheApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(m => { m.CustomSchemaIds(s => s.FullName?.Replace("+", ".")); });

        builder.Services.AddTransient<ValidationMiddleware>();
        builder.Services.AddTransient<BusinessRuleMiddleware>();
        builder.Services.AddTransient<NotFoundMiddleware>();
    }
}