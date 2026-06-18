using Eventing.Web;
using Eventing.Web.Components;
using Eventing.Web.Services;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();
builder.Services.AddScoped<TokenService>();

builder.Services.AddHttpClient(
    name: HttpClientNames.EventingApi,
    configureClient: client =>
    {
        client.BaseAddress = new Uri("https+http://apiservice");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseOutputCache();

app.MapStaticAssets();

// ✅ This already handles your Blazor routing (Index.razor, Home.razor, etc.)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ❌ REMOVE this if it maps `/` as well (causing duplicate endpoints)
// app.MapDefaultEndpoints();

app.Run();
