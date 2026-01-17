using TechbodiaNotes.Api.DTOs.Notes;
using TechbodiaNotes.Api.DTOs.Common;
using TechbodiaNotes.Api.Models;
using TechbodiaNotes.Api.Repositories;

namespace TechbodiaNotes.Api.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<PaginatedResponse<NoteResponse>> GetNotesAsync(Guid userId, NotesQueryParams queryParams)
    {
        var (notes, total) = await _noteRepository.GetByUserIdAsync(userId, queryParams);

        var noteResponses = notes.Select(MapToNoteResponse);

        return PaginatedResponse<NoteResponse>.Create(noteResponses, total, queryParams.Page, queryParams.Limit);
    }

    public async Task<NoteResponse?> GetNoteByIdAsync(Guid noteId, Guid userId)
    {
        var note = await _noteRepository.GetByIdAndUserIdAsync(noteId, userId);
        return note != null ? MapToNoteResponse(note) : null;
    }

    public async Task<NoteResponse> CreateNoteAsync(Guid userId, CreateNoteRequest request)
    {
        var note = new Note
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Content = request.Content
        };

        var createdNote = await _noteRepository.CreateAsync(note);

        return MapToNoteResponse(createdNote);
    }

    public async Task<NoteResponse> UpdateNoteAsync(Guid noteId, Guid userId, UpdateNoteRequest request)
    {
        var existingNote = await _noteRepository.GetByIdAndUserIdAsync(noteId, userId);

        if (existingNote == null)
        {
            throw new KeyNotFoundException("Note not found");
        }

        existingNote.Title = request.Title.Trim();
        existingNote.Content = request.Content;

        var updatedNote = await _noteRepository.UpdateAsync(existingNote);

        return MapToNoteResponse(updatedNote);
    }

    public async Task DeleteNoteAsync(Guid noteId, Guid userId)
    {
        var note = await _noteRepository.GetByIdAndUserIdAsync(noteId, userId);

        if (note == null)
        {
            throw new KeyNotFoundException("Note not found");
        }

        await _noteRepository.DeleteAsync(noteId);
    }

    private static NoteResponse MapToNoteResponse(Note note)
    {
        return new NoteResponse
        {
            Id = note.Id,
            UserId = note.UserId,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
    }
}
