using Coders_Back.Domain.Extensions;
using Coders_Back.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("requests")]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly IRequestService _requestService;

    public RequestController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost("{projectId:guid}/accept")]
    public async Task<IActionResult> Accept([FromRoute] Guid projectId)
    {
        var result = await _requestService.Accept(projectId, User.GetUserId()!.Value);
        return result ? Ok() : BadRequest();
    }
    
    [HttpPost("{projectId:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid projectId)
    {
        var result = await _requestService.Reject(projectId, User.GetUserId()!.Value);
        return result ? Ok() : BadRequest();
    }
}