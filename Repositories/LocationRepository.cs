using System.Data;
using Dapper;
using loma_api.Models;

namespace loma_api.Repositories;

public class LocationRepository
{
    private readonly IDbConnection _db;

    public LocationRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<Guid> CreateAsync(Location location)
    {
        var sql = @"INSERT INTO locations 
                    (id, user_id, name, description, latitude, longitude, place_id, address_line, link, is_favorite, created_at, updated_at)
                    VALUES (@Id, @UserId, @Name, @Description, @Latitude, @Longitude, @PlaceId, @AddressLine, @Link, @IsFavorite, @CreatedAt, @UpdatedAt)";
        await _db.ExecuteAsync(sql, location);
        return location.Id;
    }

    public async Task<IEnumerable<Location>> GetByUserIdAsync(Guid userId)
    {
        var sql = "SELECT * FROM locations WHERE user_id = @UserId ORDER BY created_at DESC";
        return await _db.QueryAsync<Location>(sql, new { UserId = userId });
    }

    public async Task<int> DeleteAsync(Guid id, Guid userId)
    {
        var sql = "DELETE FROM locations WHERE id = @Id AND user_id = @UserId";
        return await _db.ExecuteAsync(sql, new { Id = id, UserId = userId });
    }

    public async Task<Location?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM locations WHERE id = @Id";
        return await _db.QueryFirstOrDefaultAsync<Location>(sql, new { Id = id });
    }
}
