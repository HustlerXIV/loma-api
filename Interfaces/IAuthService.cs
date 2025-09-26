using loma_api.Dtos;

namespace loma_api.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
