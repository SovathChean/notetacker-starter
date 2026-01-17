using System.ComponentModel.DataAnnotations;

namespace TechbodiaNotes.Api.DTOs.Auth;

public class LogoutRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
