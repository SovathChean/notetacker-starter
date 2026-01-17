using System.ComponentModel.DataAnnotations;

namespace TechbodiaNotes.Api.DTOs.Notes;

public class NotesQueryParams
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    public int Limit { get; set; } = 10;

    [MaxLength(200, ErrorMessage = "Search query must be at most 200 characters")]
    public string? Search { get; set; }

    public string SortBy { get; set; } = "created_at";

    public string SortOrder { get; set; } = "desc";

    // Validate sort options
    public bool IsValidSortBy()
    {
        var validOptions = new[] { "created_at", "updated_at", "title" };
        return validOptions.Contains(SortBy.ToLower());
    }

    public bool IsValidSortOrder()
    {
        var validOptions = new[] { "asc", "desc" };
        return validOptions.Contains(SortOrder.ToLower());
    }
}
