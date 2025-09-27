namespace loma_api.Models;

public class ShareToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
