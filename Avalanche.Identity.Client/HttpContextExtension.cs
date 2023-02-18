using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Avalanche.Identity.Client;

public static class HttpContextExtension
{
    public static Guid GetUserId(this HttpContext context)
    {
        var subject = context.User.GetClaim(OpenIddictConstants.Claims.Subject);

        if (subject is null) throw new Exception();

        return Guid.Parse(subject);
    }
}