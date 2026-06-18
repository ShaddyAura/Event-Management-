using Eventing.Web.Features.Register.Models;
using Eventing.Web.Features.Register.Models.Http;
using Microsoft.AspNetCore.Components;

namespace Eventing.Web.Features.Register;

public partial class RegisterPage(
    NavigationManager navigationManager,
    IHttpClientFactory clientFactory) : ComponentBase
{
    private RegisterModel _model = new();
    private bool _isLoading;
    private bool _showPassword;
    private bool _showConfirmPassword;
    private string? _errorMessage;
    private bool _successMessage;

    private void TogglePassword() => _showPassword = !_showPassword;
    private void ToggleConfirmPassword() => _showConfirmPassword = !_showConfirmPassword;

    private async Task SubmitAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
            var requestDto = new RegisterRequestDto(
                _model.Name,
                _model.Email,
                _model.Password,
                _model.ConfirmPassword
            );

            var response = await clientFactory
                .CreateClient(HttpClientNames.EventingApi)
                .PostAsJsonAsync("api/account/register", requestDto);

            if (response.IsSuccessStatusCode)
            {
                _successMessage = true;
                // Give user time to see success message then redirect to login
                await Task.Delay(2000);
                navigationManager.NavigateTo("/login", replace: true);
                return;
            }

            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemResponse>();
            if (problem?.Errors is { Count: > 0 })
                _errorMessage = string.Join(" ", problem.Errors.SelectMany(e => e.Value));
            else
                _errorMessage = "Registration failed. Please check your details and try again.";
        }
        catch
        {
            _errorMessage = "Something went wrong. Please try again.";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private sealed class ValidationProblemResponse
    {
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
