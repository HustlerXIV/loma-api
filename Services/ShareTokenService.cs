using System.Text.Json;
using loma_api.Dtos;
using loma_api.Models;
using loma_api.Repositories;

namespace loma_api.Services;

public class ShareTokenService : IShareTokenService
{
    private readonly ShareTokenRepository _repo;
    private readonly LocationRepository _locationRepo;
    private readonly IAuditLogService _auditLog;

    public ShareTokenService(
        ShareTokenRepository repo,
        LocationRepository locationRepo,
        IAuditLogService auditLog)
    {
        _repo = repo;
        _locationRepo = locationRepo;
        _auditLog = auditLog;
    }

    public async Task<ShareTokenResponse> GenerateAsync(Guid userId, CreateShareTokenRequest request)
    {
        var token = new ShareToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LocationId = request.LocationId,
            Token = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        await _repo.CreateAsync(token);

        await _auditLog.LogAsync(userId, "SHARETOKEN_GENERATED", "User generated a share token", 
            new { token.Id, token.LocationId, token.ExpiresAt });

        return new ShareTokenResponse
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<RedirectResponse?> RedirectAsync(string token)
    {
        var shareToken = await _repo.GetValidTokenAsync(token);
        if (shareToken == null)
        {
            await _auditLog.LogAsync(null, "SHARETOKEN_REDIRECT_FAILED", "Invalid or expired token", new { token });
            return null;
        }

        var location = await _locationRepo.GetByIdAsync(shareToken.LocationId);
        if (location == null)
        {
            await _auditLog.LogAsync(shareToken.UserId, "SHARETOKEN_REDIRECT_FAILED", "Location not found", new { token });
            return null;
        }

        await _repo.MarkAsUsedAsync(token);

        await _auditLog.LogAsync(shareToken.UserId, "SHARETOKEN_REDIRECT_SUCCESS", "Token redeemed", 
            new { shareToken.Id, shareToken.LocationId });

        return new RedirectResponse
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Link = location.Link
        };
    }

    public async Task<bool> RevokeAsync(string token, Guid userId)
    {
        var rows = await _repo.RevokeAsync(token, userId);

        if (rows > 0)
        {
            await _auditLog.LogAsync(userId, "SHARETOKEN_REVOKED", "User revoked a share token", new { token });
            return true;
        }

        await _auditLog.LogAsync(userId, "SHARETOKEN_REVOKE_FAILED", "Failed to revoke token", new { token });
        return false;
    }
}
