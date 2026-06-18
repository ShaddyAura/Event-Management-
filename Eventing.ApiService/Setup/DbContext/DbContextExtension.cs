using Eventing.ApiService.Data;
using Eventing.ApiService.Data.Seeders;
using Microsoft.EntityFrameworkCore;

namespace Eventing.ApiService.Setup.DbContext;

public static class DbContextExtension
{
    /// <summary>
    /// Seeds roles and default admin user. 
    /// Run 'dotnet ef database update' via CLI before starting the app.
    /// </summary>
    public static async Task SeedDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EventingDbContext>();

        var ct = CancellationToken.None;

        // Roles must be seeded before users
        await RolesSeeder.SeedAsync(dbContext, ct);
        await UserSeeder.SeedAsync(dbContext, ct);
        await EventSeeder.SeedAsync(dbContext, ct);
    }
}
