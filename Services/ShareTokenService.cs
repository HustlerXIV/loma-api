using System.Text.Json;
using loma_api.Dtos;
using loma_api.Models;
using loma_api.Repositories;

namespace loma_api.Services;

public class ShareTokenService : IShareTokenService
{
    private readonly ShareTokenRepository _repo;
    private readonly LocationRepository _locationRepo;

    public ShareTokenService(ShareTokenRepository repo, LocationRepository locationRepo)
    {
        _repo = repo;
        _locationRepo = locationRepo;
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

        return new ShareTokenResponse
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<RedirectResponse?> RedirectAsync(string token)
    {
        var shareToken = await _repo.GetValidTokenAsync(token);
        if (shareToken == null) return null;

        var location = await _locationRepo.GetByIdAsync(shareToken.LocationId);
        if (location == null) return null;

        await _repo.MarkAsUsedAsync(token);

        return new RedirectResponse
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Link = location.Link
        };
    }
}
