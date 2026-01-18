using Microsoft.EntityFrameworkCore;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;
using TechbodiaNotes.Api.DTOs.Notes;

namespace TechbodiaNotes.Api.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NoteRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Note?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        return await _dbContext.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
    }

    public async Task<(IEnumerable<Note> Notes, int Total)> GetByUserIdAsync(Guid userId, NotesQueryParams queryParams)
    {
        var query = _dbContext.Notes
            .AsNoTracking()
            .Where(n => n.UserId == userId);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var searchTerm = queryParams.Search.ToLower();
            query = query.Where(n =>
                n.Title.ToLower().Contains(searchTerm) ||
                n.Content.ToLower().Contains(searchTerm));
        }

        // Get total count before pagination
        var total = await query.CountAsync();

        // Apply sorting
        query = queryParams.SortBy.ToLower() switch
        {
            "title" => queryParams.SortOrder.ToLower() == "asc"
                ? query.OrderBy(n => n.Title)
                : query.OrderByDescending(n => n.Title),
            "updated_at" or "updatedat" => queryParams.SortOrder.ToLower() == "asc"
                ? query.OrderBy(n => n.UpdatedAt)
                : query.OrderByDescending(n => n.UpdatedAt),
            "created_at" or "createdat" or _ => queryParams.SortOrder.ToLower() == "asc"
                ? query.OrderBy(n => n.CreatedAt)
                : query.OrderByDescending(n => n.CreatedAt)
        };

        // Apply pagination
        var offset = (queryParams.Page - 1) * queryParams.Limit;
        var notes = await query
            .Skip(offset)
            .Take(queryParams.Limit)
            .ToListAsync();

        return (notes, total);
    }

    public async Task<Note> CreateAsync(Note note)
    {
        note.Id = Guid.NewGuid();
        note.CreatedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;

        _dbContext.Notes.Add(note);
        await _dbContext.SaveChangesAsync();

        return note;
    }

    public async Task<Note> UpdateAsync(Note note)
    {
        var entity = await _dbContext.Notes
            .FirstOrDefaultAsync(n => n.Id == note.Id && n.UserId == note.UserId);

        if (entity == null)
        {
            throw new InvalidOperationException($"Note with Id {note.Id} not found");
        }

        entity.Title = note.Title;
        entity.Content = note.Content;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Notes.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Notes.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<bool> BelongsToUserAsync(Guid noteId, Guid userId)
    {
        return await _dbContext.Notes
            .AsNoTracking()
            .AnyAsync(n => n.Id == noteId && n.UserId == userId);
    }
}
