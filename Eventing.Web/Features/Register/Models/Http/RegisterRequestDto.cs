namespace Eventing.Web.Features.Register.Models.Http;

public sealed class RegisterRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;

    public RegisterRequestDto() { }

    public RegisterRequestDto(string name, string email, string password, string confirmPassword)
    {
        Name = name;
        Email = email;
        Password = password;
        ConfirmPassword = confirmPassword;
    }
}
