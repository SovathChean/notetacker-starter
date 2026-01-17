using System.ComponentModel.DataAnnotations;

namespace TechbodiaNotes.Api.DTOs.Notes;

public class CreateNoteRequest
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [MaxLength(200, ErrorMessage = "Title must be at most 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;
}
