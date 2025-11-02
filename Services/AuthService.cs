using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using loma_api.Dtos;
using loma_api.Interfaces;
using loma_api.Models;
using loma_api.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace loma_api.Services;

public class AuthService : IAuthService
{
    private readonly UserRepository _userRepo;
    private readonly IConfiguration _config;

    public AuthService(UserRepository userRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _config = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new Exception("Invalid credentials");

        var token = GenerateJwtToken(user);

        return new LoginResponse
        {
            Token = token,
            FullName = user.FullName
        };
    }

    // -------------------- REGISTER --------------------
    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("Email already registered");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Password = passwordHash,
            FullName = request.FullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Provider = "local"
        };

        await _userRepo.CreateAsync(newUser);

        return "User registered successfully";
    }

    // -------------------- GOOGLE LOGIN --------------------
    public async Task<LoginResponse> GoogleLoginAsync(string googleToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
        var email = payload.Email;
        var name = payload.Name;
        var providerId = payload.Subject;

        var user = await _userRepo.GetByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = name,
                Provider = "google",
                ProviderId = providerId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _userRepo.CreateAsync(user);
        }

        var token = GenerateJwtToken(user);

        return new LoginResponse
        {
            Token = token,
            FullName = user.FullName
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("fullName", user.FullName),
            new Claim("provider", user.Provider ?? "local")
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
