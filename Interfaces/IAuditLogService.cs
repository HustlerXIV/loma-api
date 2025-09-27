namespace loma_api.Services;

public interface IAuditLogService
{
    Task LogAsync(Guid? userId, string action, string? purpose = null, object? metadata = null);
}
