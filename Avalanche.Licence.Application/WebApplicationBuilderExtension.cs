using System.Reflection;
using Avalanche.Application;
using Microsoft.AspNetCore.Builder;

namespace Avalanche.Licence.Application;

public static class WebApplicationBuilderExtension
{
    public static void UseCalendar(this WebApplicationBuilder builder)
    {
        builder.Services.UseAvalanche(m => { m.UseFeatures(Assembly.GetExecutingAssembly()); });
    }
}