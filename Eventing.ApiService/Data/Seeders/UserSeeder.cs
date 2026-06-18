using Eventing.ApiService.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Eventing.ApiService.Data.Seeders;

public static class UserSeeder
{
    // Fixed admin credentials — change password after first login
    private static readonly Guid AdminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private const string AdminEmail    = "admin@meetupx.com";
    private const string AdminName     = "MeetupX Admin";
    private const string AdminPassword = "Admin@12345";

    public static async Task SeedAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        var userManager = dbContext.GetService<UserManager<IdentityUser<Guid>>>();

        // Skip if admin already exists
        if (await userManager.FindByIdAsync(AdminId.ToString()) is not null)
            return;

        // 1. Create the IdentityUser
        var adminUser = new IdentityUser<Guid>
        {
            Id                 = AdminId,
            UserName           = AdminId.ToString(),
            Email              = AdminEmail,
            EmailConfirmed     = true,   // skip email confirmation for seeded admin
            NormalizedEmail    = AdminEmail.ToUpperInvariant(),
            NormalizedUserName = AdminId.ToString().ToUpperInvariant()
        };

        var result = await userManager.CreateAsync(adminUser, AdminPassword);
        if (!result.Succeeded)
            throw new Exception($"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        // 2. Assign Admin role
        await userManager.AddToRoleAsync(adminUser, "Admin");

        // 3. Create the Profile row
        var profile = new Profile { Id = AdminId, Name = AdminName };
        await dbContext.Set<Profile>().AddAsync(profile, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
