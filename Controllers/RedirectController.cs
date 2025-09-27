using loma_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace loma_api.Controllers;

[ApiController]
[Route("redirect")]
public class RedirectController : ControllerBase
{
    private readonly IShareTokenService _service;

    public RedirectController(IShareTokenService service)
    {
        _service = service;
    }

    [HttpGet("{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> RedirectToLocation(string token)
    {
        var result = await _service.RedirectAsync(token);
        if (result == null)
            return BadRequest(new { message = "Invalid or expired token" });

        return Ok(result);
    }
}
