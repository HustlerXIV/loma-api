using System.Data;
using Dapper;
using loma_api.Models;

namespace loma_api.Repositories;

public class ShareTokenRepository
{
    private readonly IDbConnection _db;

    public ShareTokenRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task CreateAsync(ShareToken token)
    {
        var sql = @"INSERT INTO share_tokens 
                   (id, user_id, location_id, token, created_at, expires_at, used_at)
                   VALUES (@Id, @UserId, @LocationId, @Token, @CreatedAt, @ExpiresAt, @UsedAt)";
        await _db.ExecuteAsync(sql, token);
    }

    public async Task<ShareToken?> GetValidTokenAsync(string token)
    {
        var sql = @"SELECT * FROM share_tokens 
                    WHERE token = @Token 
                      AND used_at IS NULL 
                      AND expires_at > SYSDATETIME()";
        return await _db.QueryFirstOrDefaultAsync<ShareToken>(sql, new { Token = token });
    }

    public async Task MarkAsUsedAsync(string token)
    {
        var sql = "UPDATE share_tokens SET used_at = SYSDATETIME() WHERE token = @Token AND used_at IS NULL";
        await _db.ExecuteAsync(sql, new { Token = token });
    }

    public async Task<int> RevokeAsync(string token, Guid userId)
    {
        var sql = @"UPDATE share_tokens 
                    SET revoked_at = SYSDATETIME()
                    WHERE token = @Token 
                    AND user_id = @UserId
                    AND used_at IS NULL
                    AND revoked_at IS NULL";
        return await _db.ExecuteAsync(sql, new { Token = token, UserId = userId });
    }
}
