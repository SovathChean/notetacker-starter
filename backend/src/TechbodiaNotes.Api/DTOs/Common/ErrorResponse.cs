namespace TechbodiaNotes.Api.DTOs.Common;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ErrorResponse Create(string message)
    {
        return new ErrorResponse { Message = message };
    }

    public static ErrorResponse Create(string message, Dictionary<string, string[]> errors)
    {
        return new ErrorResponse { Message = message, Errors = errors };
    }
}
