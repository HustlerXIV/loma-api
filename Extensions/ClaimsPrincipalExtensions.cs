using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace loma_api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userId == null)
            throw new UnauthorizedAccessException("User ID not found in token");

        return Guid.Parse(userId);
    }
}
