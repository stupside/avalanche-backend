using Microsoft.AspNetCore.Builder;

namespace Avalanche.Application.Grpc;

public static class WebApplicationExtension
{
    public static void UseAvalancheGrpc(this WebApplication application)
    {
        application.MapGrpcReflectionService();
    }
}