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

    [HttpPost("{requestId:guid}/accept")]
    public async Task<IActionResult> Accept([FromRoute] Guid requestId)
    {
        var result = await _requestService.Accept(requestId, User.GetCurrentUserId()!.Value);
        return result ? Ok() : BadRequest();
    }
    
    [HttpPost("{requestId:guid}/reject")]
    public async Task<IActionResult> Reject([FromRoute] Guid requestId)
    {
        var result = await _requestService.Reject(requestId, User.GetCurrentUserId()!.Value);
        return result ? Ok() : BadRequest();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = User.GetCurrentUserId();
        var requestOutputs = await _requestService.GetByUser(userId!.Value);
        return Ok(requestOutputs);
    }
    
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var userId = User.GetCurrentUserId();
        var requestOutputs = await _requestService.GetPendingByUser(userId!.Value);
        return Ok(requestOutputs);
    }
}