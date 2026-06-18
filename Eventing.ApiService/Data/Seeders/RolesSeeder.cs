using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Eventing.ApiService.Data.Seeders;

public static class RolesSeeder
{
    private static readonly string[] Roles = ["General", "Admin"];

    public static async Task SeedAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        var roleManager = dbContext.GetService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
