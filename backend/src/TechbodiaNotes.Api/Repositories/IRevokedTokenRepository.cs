using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public interface IRevokedTokenRepository
{
    Task<bool> IsTokenRevokedAsync(string jti);
    Task RevokeTokenAsync(RevokedToken revokedToken);
    Task CleanupExpiredTokensAsync();
}
