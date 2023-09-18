using Avalanche.Application;
using Microsoft.AspNetCore.Builder;

namespace Avalanche.Sales.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseSales(this WebApplicationBuilder builder)
    {
        builder.Services.UseAvalanche(m => { m.UseFeatures(typeof(WebApplicationBuilderExtension).Assembly); });
    }
}