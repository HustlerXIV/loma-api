using loma_api.Dtos;

namespace loma_api.Interfaces;

public interface ILocationService
{
    Task<LocationResponse> CreateAsync(Guid userId, CreateLocationRequest request);
    Task<IEnumerable<LocationResponse>> GetAllAsync(Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<LocationResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<LocationResponse?> UpdateAsync(Guid id, Guid userId, UpdateLocationRequest request);
}
