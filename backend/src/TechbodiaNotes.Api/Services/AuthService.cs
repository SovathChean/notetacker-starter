using System.IdentityModel.Tokens.Jwt;
using TechbodiaNotes.Api.DTOs.Auth;
using TechbodiaNotes.Api.Models;
using TechbodiaNotes.Api.Repositories;

namespace TechbodiaNotes.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRevokedTokenRepository _revokedTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IRevokedTokenRepository revokedTokenRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _revokedTokenRepository = revokedTokenRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException("Email is already registered");
        }

        // Check if username exists
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
        {
            throw new InvalidOperationException("Username is already taken");
        }

        // Create user
        var user = new User
        {
            Email = request.Email.ToLower(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        user = await _userRepository.CreateAsync(user);

        // Generate tokens
        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var identifier = request.Identifier.Trim().ToLower();

        // Determine if identifier is email or username
        User? user;
        if (identifier.Contains('@'))
        {
            user = await _userRepository.GetByEmailAsync(identifier);
        }
        else
        {
            user = await _userRepository.GetByUsernameAsync(identifier);
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email/username or password");
        }

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        if (!storedToken.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token is expired or revoked");
        }

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Revoke old token
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);
        await _refreshTokenRepository.RevokeAsync(refreshToken, newRefreshToken.Token);

        // Save new refresh token
        await _refreshTokenRepository.CreateAsync(newRefreshToken);

        var expirationMinutes = int.Parse(_configuration.GetSection("JwtSettings")["AccessTokenExpirationMinutes"] ?? "15");

        return new AuthResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = newRefreshToken.Token,
            ExpiresIn = expirationMinutes * 60,
            User = MapToUserDto(user)
        };
    }

    public async Task LogoutAsync(string refreshToken, string accessToken)
    {
        // Revoke refresh token
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken != null && storedToken.IsActive)
        {
            await _refreshTokenRepository.RevokeAsync(refreshToken);
        }

        // Revoke access token by adding its JTI to the blacklist
        if (!string.IsNullOrEmpty(accessToken))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                var jti = token.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
                var exp = token.ValidTo;
                var userId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (!string.IsNullOrEmpty(jti) && Guid.TryParse(userId, out var userGuid))
                {
                    var revokedToken = new RevokedToken
                    {
                        Id = Guid.NewGuid(),
                        Jti = jti,
                        UserId = userGuid,
                        ExpiresAt = exp,
                        RevokedAt = DateTime.UtcNow
                    };

                    await _revokedTokenRepository.RevokeTokenAsync(revokedToken);
                }
            }
            catch
            {
                // If token parsing fails, ignore - the refresh token is already revoked
            }
        }
    }

    public async Task<bool> IsTokenRevokedAsync(string jti)
    {
        return await _revokedTokenRepository.IsTokenRevokedAsync(jti);
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.CreateAsync(refreshToken);

        var expirationMinutes = int.Parse(_configuration.GetSection("JwtSettings")["AccessTokenExpirationMinutes"] ?? "15");

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = expirationMinutes * 60,
            User = MapToUserDto(user)
        };
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
