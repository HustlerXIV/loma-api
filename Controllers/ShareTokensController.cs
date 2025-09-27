using loma_api.Dtos;
using loma_api.Extensions;
using loma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace loma_api.Controllers;

[ApiController]
[Route("shareTokens")]
[Authorize]
public class ShareTokensController : ControllerBase
{
    private readonly IShareTokenService _service;

    public ShareTokensController(IShareTokenService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Generate(CreateShareTokenRequest request)
    {
        var result = await _service.GenerateAsync(User.GetUserId(), request);
        return Ok(result);
    }

    [HttpPost("{token}/revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke(string token)
    {
        var success = await _service.RevokeAsync(token, User.GetUserId());
        if (!success) 
            return BadRequest(new { message = "Token invalid, already used, or already revoked" });

        return Ok(new { message = "Token revoked successfully" });
    }
}
