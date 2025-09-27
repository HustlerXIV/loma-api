using loma_api.Dtos;
using loma_api.Models;
using loma_api.Repositories;
using loma_api.Interfaces;

namespace loma_api.Services;

public class LocationService : ILocationService
{
    private readonly LocationRepository _repo;

    public LocationService(LocationRepository repo)
    {
        _repo = repo;
    }

    public async Task<LocationResponse> CreateAsync(Guid userId, CreateLocationRequest request)
    {
        var location = new Location
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PlaceId = request.PlaceId,
            AddressLine = request.AddressLine,
            Link = request.Link,
            IsFavorite = request.IsFavorite,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.CreateAsync(location);

        return new LocationResponse
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            PlaceId = location.PlaceId,
            AddressLine = location.AddressLine,
            Link = location.Link,
            IsFavorite = location.IsFavorite
        };
    }

    public async Task<IEnumerable<LocationResponse>> GetAllAsync(Guid userId)
    {
        var locations = await _repo.GetByUserIdAsync(userId);
        return locations.Select(l => new LocationResponse
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            PlaceId = l.PlaceId,
            AddressLine = l.AddressLine,
            Link = l.Link,
            IsFavorite = l.IsFavorite
        });
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var rows = await _repo.DeleteAsync(id, userId);
        return rows > 0;
    }
}
