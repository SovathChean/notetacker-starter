using Microsoft.EntityFrameworkCore;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public class RevokedTokenRepository : IRevokedTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RevokedTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsTokenRevokedAsync(string jti)
    {
        return await _context.RevokedTokens
            .AnyAsync(t => t.Jti == jti);
    }

    public async Task RevokeTokenAsync(RevokedToken revokedToken)
    {
        // Check if already revoked
        var exists = await _context.RevokedTokens
            .AnyAsync(t => t.Jti == revokedToken.Jti);

        if (!exists)
        {
            _context.RevokedTokens.Add(revokedToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RevokedTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RevokedTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
