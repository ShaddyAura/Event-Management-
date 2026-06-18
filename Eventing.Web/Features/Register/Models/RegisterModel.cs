using System.ComponentModel.DataAnnotations;

namespace Eventing.Web.Features.Register.Models;

public sealed class RegisterModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [RegularExpression(@"^[\p{L}]+([ '\-\.][\p{L}]+)*$", ErrorMessage = "Please enter a valid full name.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(10, ErrorMessage = "Password must be at least 10 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool AgreeToTerms { get; set; }
}
