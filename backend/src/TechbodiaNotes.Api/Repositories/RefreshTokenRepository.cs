using Microsoft.EntityFrameworkCore;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        refreshToken.Id = Guid.NewGuid();
        refreshToken.CreatedAt = DateTime.UtcNow;

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }

    public async Task RevokeAsync(string token, string? replacedByToken = null)
    {
        var entity = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (entity != null)
        {
            entity.RevokedAt = DateTime.UtcNow;
            entity.ReplacedByToken = replacedByToken;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteExpiredAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-1);
        var expiredTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.ExpiresAt < cutoffDate || rt.RevokedAt != null)
            .ToListAsync();

        _dbContext.RefreshTokens.RemoveRange(expiredTokens);
        await _dbContext.SaveChangesAsync();
    }
}
