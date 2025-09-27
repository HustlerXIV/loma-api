namespace loma_api.Models;

public class AuditLog
{
    public long Id { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Purpose { get; set; }
    public string? Metadata { get; set; }
    public DateTime OccurredAt { get; set; }
}
