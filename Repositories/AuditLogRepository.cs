using System.Data;
using Dapper;
using loma_api.Models;

namespace loma_api.Repositories;

public class AuditLogRepository
{
    private readonly IDbConnection _db;

    public AuditLogRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(AuditLog log)
    {
        var sql = @"INSERT INTO audit_logs (user_id, action, purpose, metadata, occurred_at)
                    VALUES (@UserId, @Action, @Purpose, @Metadata, @OccurredAt)";
        return await _db.ExecuteAsync(sql, log);
    }
}
