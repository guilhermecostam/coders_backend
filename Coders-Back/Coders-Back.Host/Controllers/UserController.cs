using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.Extensions;
using Coders_Back.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Authorize]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    //TODO: leave a project

    [HttpPut("/{id:guid}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserInput input)
    {
        input.Id = id;
        if (User.GetCurrentUserId() != input.Id) return Unauthorized();
        var success = await _userService.UpdateUser(input);
        if(success) return NoContent();
        return BadRequest();
    }

    [HttpGet("/{id}/projects")]
    public async Task<IActionResult> GetProjectsByUser([FromRoute] Guid id)
    {
        var projects = await _userService.GetProjectsByUser(id);
        return Ok(projects);
    }
}