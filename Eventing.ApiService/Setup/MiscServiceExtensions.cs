using Eventing.ApiService.Services.CurrentUser;

namespace Eventing.ApiService.Setup;

public static class MiscServiceExtensions
{
    public static void AddXMiscServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<CurrentUserService>();
    }
}