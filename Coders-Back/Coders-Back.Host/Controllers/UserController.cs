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
    private readonly IRequestService _requestService;

    public UserController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests()
    {
        var userId = User.GetUserId();
        var requestOutputs = await _requestService.GetByUser(userId!.Value);
        return Ok(requestOutputs);
    }
}