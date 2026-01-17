using Dapper;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
            @"SELECT Id, UserId, Token, ExpiresAt, CreatedAt, RevokedAt, ReplacedByToken
              FROM RefreshTokens WHERE Token = @Token",
            new { Token = token });
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<RefreshToken>(
            @"SELECT Id, UserId, Token, ExpiresAt, CreatedAt, RevokedAt, ReplacedByToken
              FROM RefreshTokens WHERE UserId = @UserId",
            new { UserId = userId });
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO RefreshTokens (Id, UserId, Token, ExpiresAt, CreatedAt)
            OUTPUT INSERTED.Id, INSERTED.UserId, INSERTED.Token, INSERTED.ExpiresAt, INSERTED.CreatedAt, INSERTED.RevokedAt, INSERTED.ReplacedByToken
            VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt)";

        refreshToken.Id = Guid.NewGuid();
        refreshToken.CreatedAt = DateTime.UtcNow;

        return await connection.QuerySingleAsync<RefreshToken>(sql, refreshToken);
    }

    public async Task RevokeAsync(string token, string? replacedByToken = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            @"UPDATE RefreshTokens SET RevokedAt = @RevokedAt, ReplacedByToken = @ReplacedByToken
              WHERE Token = @Token",
            new { Token = token, RevokedAt = DateTime.UtcNow, ReplacedByToken = replacedByToken });
    }

    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE RefreshTokens SET RevokedAt = @RevokedAt WHERE UserId = @UserId AND RevokedAt IS NULL",
            new { UserId = userId, RevokedAt = DateTime.UtcNow });
    }

    public async Task DeleteExpiredAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            "DELETE FROM RefreshTokens WHERE ExpiresAt < @Now OR RevokedAt IS NOT NULL",
            new { Now = DateTime.UtcNow.AddDays(-1) });
    }
}
