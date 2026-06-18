using Eventing.Web.Features.Login.Models;
using Eventing.Web.Features.Login.Models.Http;
using Eventing.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace Eventing.Web.Features.Login;

public partial class LoginPage(
    ProtectedLocalStorage protectedLocalStorage,
    ProtectedSessionStorage protectedSessionStorage,
    TokenService tokenService,
    IHttpClientFactory clientFactory,
    IJSRuntime js) : ComponentBase
{
    private LoginModel _model = new();
    private bool _isLoading;
    private bool _showPassword;
    private string? _errorMessage;

    private void TogglePassword() => _showPassword = !_showPassword;

    private async Task SubmitAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
            var requestDto = new LoginRequestDto(_model.Email, _model.Password);
            var response = await clientFactory
                .CreateClient(HttpClientNames.EventingApi)
                .PostAsJsonAsync("api/account/login", requestDto);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                ArgumentNullException.ThrowIfNull(content);

                // Decode role DIRECTLY from the raw JWT — before touching storage
                var role = TokenService.GetRoleFromToken(content.AccessToken);

                // Save token to browser storage
                if (_model.RememberMe)
                {
                    await protectedLocalStorage.SetAsync("AccessToken", content.AccessToken);
                    await protectedLocalStorage.SetAsync("ExpiresIn", content.ExpiresIn);
                }
                else
                {
                    await protectedSessionStorage.SetAsync("AccessToken", content.AccessToken);
                    await protectedSessionStorage.SetAsync("ExpiresIn", content.ExpiresIn);
                }

                tokenService.ClearCache();

                var redirect = role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true
                    ? "/dashboard/admin"
                    : "/dashboard";

                // Use JS window.location for reliable navigation after storage writes
                await js.InvokeVoidAsync("eval", $"window.location.href='{redirect}'");
                return;
            }

            _errorMessage = "Invalid email or password. Please try again.";
        }
        catch (Exception ex)
        {
            _errorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }
}
