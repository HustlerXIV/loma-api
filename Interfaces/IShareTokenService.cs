using loma_api.Dtos;

namespace loma_api.Services;

public interface IShareTokenService
{
    Task<ShareTokenResponse> GenerateAsync(Guid userId, CreateShareTokenRequest request);
    Task<RedirectResponse?> RedirectAsync(string token);
}
