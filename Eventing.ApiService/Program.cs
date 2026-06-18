using Eventing.ApiService.Data;
using Eventing.ApiService.Setup;
using Eventing.ApiService.Setup.Auth;
using Eventing.ApiService.Setup.DbContext;
using Eventing.ApiService.Setup.Emailing;
using Eventing.ApiService.Setup.Identity;
using Eventing.ApiService.Setup.JsonOptions;
using Eventing.ApiService.Setup.Jwt;
using Eventing.ApiService.Setup.OpenApi;
using Eventing.ApiService.Setup.Scalar;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddXOpenApi();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddXJsonOptions();
builder.AddRedisDistributedCache("cache");

// Register DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EventingDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors()
               .EnableSensitiveDataLogging();
    }
});

builder.Services.AddXIdentityCore();
builder.Services.AddXJwt();
builder.Services.AddXAuthentication();
builder.Services.AddXAuthorization();

builder.AddXEmailing();

builder.Services.AddXMiscServices();

var app = builder.Build();

// Run migrations + seed data on startup
await app.SeedDatabaseAsync();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseXScalar();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
