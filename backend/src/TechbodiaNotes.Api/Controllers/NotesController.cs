using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechbodiaNotes.Api.DTOs.Notes;
using TechbodiaNotes.Api.DTOs.Common;
using TechbodiaNotes.Api.Services;

namespace TechbodiaNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<NoteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotes([FromQuery] NotesQueryParams queryParams)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(ErrorResponse.Create("Invalid token"));
        }

        // Validate query params
        if (!queryParams.IsValidSortBy())
        {
            return BadRequest(ErrorResponse.Create("Invalid sortBy parameter. Valid values: createdAt, updatedAt, title"));
        }

        if (!queryParams.IsValidSortOrder())
        {
            return BadRequest(ErrorResponse.Create("Invalid sortOrder parameter. Valid values: asc, desc"));
        }

        var notes = await _noteService.GetNotesAsync(userId.Value, queryParams);
        return Ok(notes);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNote(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(ErrorResponse.Create("Invalid token"));
        }

        var note = await _noteService.GetNoteByIdAsync(id, userId.Value);
        if (note == null)
        {
            return NotFound(ErrorResponse.Create("Note not found"));
        }

        return Ok(note);
    }

    [HttpPost]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(ErrorResponse.Create("Invalid token"));
        }

        var note = await _noteService.CreateNoteAsync(userId.Value, request);
        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNote(Guid id, [FromBody] UpdateNoteRequest request)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(ErrorResponse.Create("Invalid token"));
        }

        try
        {
            var note = await _noteService.UpdateNoteAsync(id, userId.Value, request);
            return Ok(note);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ErrorResponse.Create("Note not found"));
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNote(Guid id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized(ErrorResponse.Create("Invalid token"));
        }

        try
        {
            await _noteService.DeleteNoteAsync(id, userId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ErrorResponse.Create("Note not found"));
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
            ?? User.FindFirst("sub");

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }
}
