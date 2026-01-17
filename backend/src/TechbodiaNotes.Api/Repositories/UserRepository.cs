using Dapper;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT Id, Email, Username, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT Id, Email, Username, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE Email = @Email",
            new { Email = email });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT Id, Email, Username, PasswordHash, CreatedAt, UpdatedAt FROM Users WHERE Username = @Username",
            new { Username = username });
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @Email) THEN 1 ELSE 0 END",
            new { Email = email });
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Username = @Username) THEN 1 ELSE 0 END",
            new { Username = username });
    }

    public async Task<User> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO Users (Id, Email, Username, PasswordHash, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.Email, INSERTED.Username, INSERTED.PasswordHash, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@Id, @Email, @Username, @PasswordHash, @CreatedAt, @UpdatedAt)";

        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        return await connection.QuerySingleAsync<User>(sql, user);
    }

    public async Task<User> UpdateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE Users
            SET Email = @Email, Username = @Username, PasswordHash = @PasswordHash, UpdatedAt = @UpdatedAt
            OUTPUT INSERTED.Id, INSERTED.Email, INSERTED.Username, INSERTED.PasswordHash, INSERTED.CreatedAt, INSERTED.UpdatedAt
            WHERE Id = @Id";

        user.UpdatedAt = DateTime.UtcNow;

        return await connection.QuerySingleAsync<User>(sql, user);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
    }
}
