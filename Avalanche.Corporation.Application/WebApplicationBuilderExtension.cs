using Avalanche.Application;
using Microsoft.AspNetCore.Builder;

namespace Avalanche.Corporation.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseBusiness(this WebApplicationBuilder builder)
    {
        builder.Services.UseAvalanche(m => { m.UseFeatures(typeof(WebApplicationBuilderExtension).Assembly); });
    }
}