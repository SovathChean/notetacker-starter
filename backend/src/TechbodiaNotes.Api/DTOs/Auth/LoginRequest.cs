using System.ComponentModel.DataAnnotations;

namespace TechbodiaNotes.Api.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email or username is required")]
    public string Identifier { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
