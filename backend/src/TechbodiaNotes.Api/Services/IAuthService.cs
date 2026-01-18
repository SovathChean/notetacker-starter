using TechbodiaNotes.Api.DTOs.Auth;

namespace TechbodiaNotes.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken, string accessToken);
    Task<bool> IsTokenRevokedAsync(string jti);
    Task<UserDto?> GetCurrentUserAsync(Guid userId);
}
