using Microsoft.EntityFrameworkCore;

namespace Eventing.ApiService.Data.Seeders;

public static class EventSeeder
{
    public static Task SeedAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        // Seeding is disabled — events are created via the API
        return Task.CompletedTask;
    }
}
