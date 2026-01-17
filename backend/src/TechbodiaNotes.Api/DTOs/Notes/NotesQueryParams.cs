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

    public string SortBy { get; set; } = "createdAt";

    public string SortOrder { get; set; } = "desc";

    // Validate sort options (accept both camelCase and snake_case)
    public bool IsValidSortBy()
    {
        var validOptions = new[] { "created_at", "updated_at", "title", "createdAt", "updatedAt", "createdat", "updatedat" };
        return validOptions.Contains(SortBy.ToLower());
    }

    // Normalize sort field to snake_case for database queries
    public string GetNormalizedSortBy()
    {
        return SortBy.ToLower() switch
        {
            "createdat" or "created_at" => "created_at",
            "updatedat" or "updated_at" => "updated_at",
            "title" => "title",
            _ => "created_at"
        };
    }

    public bool IsValidSortOrder()
    {
        var validOptions = new[] { "asc", "desc" };
        return validOptions.Contains(SortOrder.ToLower());
    }
}
