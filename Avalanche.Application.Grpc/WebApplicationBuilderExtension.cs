using Avalanche.Application.Grpc.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Avalanche.Application.Grpc;

public static class WebApplicationBuilderExtension
{
    public static void SetupAvalancheGrpc(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(m =>
        {
            const int mb = 1024 * 1024;

            m.Limits.Http2.InitialConnectionWindowSize = mb * 2;
            m.Limits.Http2.InitialStreamWindowSize = mb;
        });

        builder.Services.AddGrpc(m =>
        {
            m.Interceptors.Add<BusinessRuleInterceptor>();
            m.Interceptors.Add<NotFoundInterceptor>();
            m.Interceptors.Add<ValidationInterceptor>();
        });

        builder.Services.AddGrpcReflection();
    }
}