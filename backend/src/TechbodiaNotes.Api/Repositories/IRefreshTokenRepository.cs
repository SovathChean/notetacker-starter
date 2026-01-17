using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task RevokeAsync(string token, string? replacedByToken = null);
    Task RevokeAllByUserIdAsync(Guid userId);
    Task DeleteExpiredAsync();
}
