using System.Text.Json;
using loma_api.Models;
using loma_api.Repositories;

namespace loma_api.Services;

public class AuditLogService : IAuditLogService
{
    private readonly AuditLogRepository _repo;

    public AuditLogService(AuditLogRepository repo)
    {
        _repo = repo;
    }

    public async Task LogAsync(Guid? userId, string action, string? purpose = null, object? metadata = null)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            Purpose = purpose,
            Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
            OccurredAt = DateTime.UtcNow
        };

        await _repo.CreateAsync(log);
    }
}
