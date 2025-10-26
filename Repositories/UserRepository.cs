using System.Data;
using Dapper;
using loma_api.Models;

namespace loma_api.Repositories;

public class UserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM users WHERE email = @Email AND is_active = TRUE";
        return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<Guid> CreateAsync(User user)
    {
        var sql = @"INSERT INTO users (id, email, password, full_name, is_active, created_at, updated_at)
                    VALUES (@Id, @Email, @Password, @FullName, @IsActive, @CreatedAt, @UpdatedAt)";
        await _db.ExecuteAsync(sql, user);
        return user.Id;
    }
}
