using Dapper;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;
using TechbodiaNotes.Api.DTOs.Notes;

namespace TechbodiaNotes.Api.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NoteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Note>(
            "SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<Note?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Note>(
            "SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
    }

    public async Task<(IEnumerable<Note> Notes, int Total)> GetByUserIdAsync(Guid userId, NotesQueryParams queryParams)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Build the WHERE clause
        var whereClause = "WHERE UserId = @UserId";
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            whereClause += " AND (Title LIKE @Search OR Content LIKE @Search)";
            parameters.Add("Search", $"%{queryParams.Search}%");
        }

        // Build ORDER BY clause
        var orderColumn = queryParams.SortBy.ToLower() switch
        {
            "title" => "Title",
            "updated_at" => "UpdatedAt",
            _ => "CreatedAt"
        };
        var orderDirection = queryParams.SortOrder.ToLower() == "asc" ? "ASC" : "DESC";

        // Get total count
        var countSql = $"SELECT COUNT(*) FROM Notes {whereClause}";
        var total = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        // Get paginated data
        var offset = (queryParams.Page - 1) * queryParams.Limit;
        parameters.Add("Offset", offset);
        parameters.Add("Limit", queryParams.Limit);

        var dataSql = $@"
            SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
            FROM Notes
            {whereClause}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

        var notes = await connection.QueryAsync<Note>(dataSql, parameters);

        return (notes, total);
    }

    public async Task<Note> CreateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            INSERT INTO Notes (Id, UserId, Title, Content, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.Id, INSERTED.UserId, INSERTED.Title, INSERTED.Content, INSERTED.CreatedAt, INSERTED.UpdatedAt
            VALUES (@Id, @UserId, @Title, @Content, @CreatedAt, @UpdatedAt)";

        note.Id = Guid.NewGuid();
        note.CreatedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;

        return await connection.QuerySingleAsync<Note>(sql, note);
    }

    public async Task<Note> UpdateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
            UPDATE Notes
            SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt
            OUTPUT INSERTED.Id, INSERTED.UserId, INSERTED.Title, INSERTED.Content, INSERTED.CreatedAt, INSERTED.UpdatedAt
            WHERE Id = @Id AND UserId = @UserId";

        note.UpdatedAt = DateTime.UtcNow;

        return await connection.QuerySingleAsync<Note>(sql, note);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Notes WHERE Id = @Id", new { Id = id });
    }

    public async Task<bool> BelongsToUserAsync(Guid noteId, Guid userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM Notes WHERE Id = @Id AND UserId = @UserId) THEN 1 ELSE 0 END",
            new { Id = noteId, UserId = userId });
    }
}
