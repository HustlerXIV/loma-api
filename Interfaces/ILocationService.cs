using loma_api.Dtos;

namespace loma_api.Interfaces;

public interface ILocationService
{
    Task<LocationResponse> CreateAsync(Guid userId, CreateLocationRequest request);
    Task<IEnumerable<LocationResponse>> GetAllAsync(Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
