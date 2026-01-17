using TechbodiaNotes.Api.Models;
using TechbodiaNotes.Api.DTOs.Notes;

namespace TechbodiaNotes.Api.Repositories;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(Guid id);
    Task<Note?> GetByIdAndUserIdAsync(Guid id, Guid userId);
    Task<(IEnumerable<Note> Notes, int Total)> GetByUserIdAsync(Guid userId, NotesQueryParams queryParams);
    Task<Note> CreateAsync(Note note);
    Task<Note> UpdateAsync(Note note);
    Task DeleteAsync(Guid id);
    Task<bool> BelongsToUserAsync(Guid noteId, Guid userId);
}
