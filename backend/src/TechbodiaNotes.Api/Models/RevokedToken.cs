namespace TechbodiaNotes.Api.Models;

public class RevokedToken
{
    public Guid Id { get; set; }
    public string Jti { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime RevokedAt { get; set; }
}
