using loma_api.Dtos;
using loma_api.Interfaces;
using loma_api.Models;
using loma_api.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace loma_api.Services;

public class ShareTokenService : IShareTokenService
{
    private readonly ShareTokenRepository _repo;
    private readonly LocationRepository _locationRepo;
    private readonly ILocationService _locationService;
    private readonly IConfiguration _config;

    public ShareTokenService(
        ShareTokenRepository repo,
        LocationRepository locationRepo,
        ILocationService locationService,
        IConfiguration config
        )
    {
        _repo = repo;
        _locationRepo = locationRepo;
        _locationService = locationService;
        _config = config;
    }

   public async Task<ShareTokenResponse> GenerateAsync(Guid userId, CreateShareTokenRequest request)
    {
        var jwtToken = GenerateShareJwtToken(userId, request.LocationId);

        var token = new ShareToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LocationId = request.LocationId,
            Token = jwtToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        var location = await _locationService.GetByIdAsync(request.LocationId, userId);
        if (location == null)
            throw new Exception("Location not found or unauthorized.");

        await _repo.CreateAsync(token);

        return new ShareTokenResponse
        {
            Name = location.Name,
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<RedirectResponse?> RedirectAsync(string token)
    {
        var shareToken = await _repo.GetValidTokenAsync(token);
        if (shareToken == null)
        {
            return null;
        }

        var location = await _locationRepo.GetByIdAsync(shareToken.LocationId);
        if (location == null)
        {
            return null;
        }

        await _repo.MarkAsUsedAsync(token);

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
            return true;
        }

        return false;
    }

    private string GenerateShareJwtToken(Guid userId, Guid locationId)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("locationId", locationId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
