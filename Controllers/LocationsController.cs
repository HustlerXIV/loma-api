using loma_api.Dtos;
using loma_api.Extensions;
using loma_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace loma_api.Controllers;

[ApiController]
[Route("locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _service;

    public LocationsController(ILocationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLocationRequest request)
    {
        var result = await _service.CreateAsync(User.GetUserId(), request);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationResponse>>> GetAll()
    {
        var result = await _service.GetAllAsync(User.GetUserId());
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id, User.GetUserId());
        if (!success) return NotFound(new { message = "Location not found or not owned by user" });
        return Ok(new { message = "Location deleted" });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var location = await _service.GetByIdAsync(id, userId);
        if (location == null)
            return NotFound();

        return Ok(location);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationRequest request)
    {
        var userId = User.GetUserId();
        var updated = await _service.UpdateAsync(id, userId, request);

        if (updated == null)
            return NotFound();

        return Ok(updated);
    }
}
