using System.Security.Claims;

namespace Eventing.ApiService.Setup.Auth;

public static class AuthorizationExtension
{
    public static void AddXAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim(ClaimTypes.Role, "Admin"));
        });
    }
}
