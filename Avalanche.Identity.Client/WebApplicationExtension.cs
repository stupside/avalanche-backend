using Microsoft.AspNetCore.Builder;

namespace Avalanche.Identity.Client;

public static class WebApplicationExtension
{
    public static void UseAvalancheIdentityClient(this WebApplication application)
    {
        application.UseAuthentication();
        application.UseAuthorization();
    }
}