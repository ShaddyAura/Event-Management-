using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Eventing.Web.Services;

/// <summary>
/// Reads the JWT from ProtectedLocalStorage / ProtectedSessionStorage
/// and exposes claims. Requires the API to issue plain signed JWTs (JWS, not JWE).
/// </summary>
public class TokenService(
    ProtectedLocalStorage localStore,
    ProtectedSessionStorage sessionStore)
{
    private string? _cachedToken;

    // ASP.NET Identity emits roles under this long URI claim type
    private const string AspNetRoleClaim =
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

    // Short-form claim keys sometimes used
    private static readonly string[] RoleClaimKeys =
    [
        ClaimTypes.Role,          // http://schemas.microsoft.com/ws/2008/06/identity/claims/role
        AspNetRoleClaim,
        "role",
        "roles"
    ];

    public async Task<string?> GetTokenAsync()
    {
        if (_cachedToken is not null) return _cachedToken;

        try
        {
            var local = await localStore.GetAsync<string>("AccessToken");
            if (local.Success && !string.IsNullOrWhiteSpace(local.Value))
            {
                _cachedToken = local.Value;
                return _cachedToken;
            }

            var session = await sessionStore.GetAsync<string>("AccessToken");
            if (session.Success && !string.IsNullOrWhiteSpace(session.Value))
            {
                _cachedToken = session.Value;
                return _cachedToken;
            }
        }
        catch
        {
            // Storage not yet available (pre-render) — return null
        }

        return null;
    }

    public async Task<ClaimsPrincipal?> GetUserAsync()
    {
        var token = await GetTokenAsync();
        if (token is null) return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                // Token might be a JWE (encrypted) — cannot read without the key on the client.
                // Solution: Remove EncryptingKey from API appsettings.json.
                return null;
            }

            var jwt = handler.ReadJwtToken(token);

            if (jwt.ValidTo < DateTime.UtcNow) return null;

            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetRoleAsync()
    {
        var user = await GetUserAsync();
        if (user is null) return null;

        // Try every known role claim key
        foreach (var key in RoleClaimKeys)
        {
            var val = user.FindFirstValue(key);
            if (!string.IsNullOrWhiteSpace(val)) return val;
        }

        return null;
    }

    public async Task<bool> IsAdminAsync()
    {
        var role = await GetRoleAsync();
        return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string?> GetNameAsync()
    {
        var user = await GetUserAsync();
        if (user is null) return null;

        // full_name is our custom claim added in AccountController login
        return user.FindFirstValue("full_name")
            ?? user.FindFirstValue(ClaimTypes.GivenName)
            ?? user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("name");
    }

    public async Task<string?> GetEmailAsync()
    {
        var user = await GetUserAsync();
        if (user is null) return null;

        return user.FindFirstValue(ClaimTypes.Email)
            ?? user.FindFirstValue("email");
    }

    public async Task<string?> GetUserIdAsync()
    {
        var user = await GetUserAsync();
        return user?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user?.FindFirstValue("sub");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetUserAsync();
        return user is not null;
    }

    public void ClearCache() => _cachedToken = null;

    /// <summary>
    /// Decode role directly from a raw JWT string — no storage needed.
    /// Use this immediately after login before storage is readable.
    /// </summary>
    public static string? GetRoleFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            foreach (var key in new[]
            {
                ClaimTypes.Role,
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                "role", "roles"
            })
            {
                var val = user.FindFirstValue(key);
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
        }
        catch { }
        return null;
    }

    public async Task LogoutAsync()
    {
        _cachedToken = null;
        try { await localStore.DeleteAsync("AccessToken"); } catch { }
        try { await localStore.DeleteAsync("ExpiresIn"); } catch { }
        try { await sessionStore.DeleteAsync("AccessToken"); } catch { }
        try { await sessionStore.DeleteAsync("ExpiresIn"); } catch { }
    }
}
