using TechbodiaNotes.Api.DTOs.Notes;
using TechbodiaNotes.Api.DTOs.Common;

namespace TechbodiaNotes.Api.Services;

public interface INoteService
{
    Task<PaginatedResponse<NoteResponse>> GetNotesAsync(Guid userId, NotesQueryParams queryParams);
    Task<NoteResponse?> GetNoteByIdAsync(Guid noteId, Guid userId);
    Task<NoteResponse> CreateNoteAsync(Guid userId, CreateNoteRequest request);
    Task<NoteResponse> UpdateNoteAsync(Guid noteId, Guid userId, UpdateNoteRequest request);
    Task DeleteNoteAsync(Guid noteId, Guid userId);
}
