using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
    Guid? ValidateAccessToken(string token);
}
