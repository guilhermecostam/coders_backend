using System.Security.Claims;
using Coders_Back.Domain.Extensions;
using Coders_Back.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IRequestService _requestService;
    public ProjectController(IProjectService projectService, IRequestService requestService)
    {
        _projectService = projectService;
        _requestService = requestService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _projectService.GetAll());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var project = await _projectService.GetById(id);
        return project is not null ? Ok(project) : NotFound();
    }
    
    [HttpGet]
    [Route("{projectId:guid}/collaborators")]
    public async Task<IActionResult> GetCollaboratorsByProjectId(Guid projectId)
    {
        var collaborators = await _projectService.GetCollaboratorsByProject(projectId);
        return Ok(collaborators);
    }

    [HttpDelete]
    [Route("collaborators/{collaboratorId:guid}")]
    public async Task<IActionResult> DeleteCollaborator(Guid collaboratorId)
    {
        var subClaim = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null) return BadRequest();
        Guid.TryParse(subClaim.Value, out var userId);
        
        await _projectService.DeleteCollaborator(collaboratorId, userId);
        return NoContent();
    }
    
    [HttpGet("{projectId:guid}/requests")]
    public async Task<IActionResult> GetRequests([FromRoute] Guid projectId)
    {
        var requestOutputs = await _requestService.GetByProject(projectId);
        return Ok(requestOutputs);
    }
    
    [HttpPost("{projectId:guid}/requests")]
    public async Task<IActionResult> Create([FromBody] Guid projectId)
    {
        var userId = User.GetUserId();
        var result = await _requestService.Create(projectId, userId!.Value);
        return result.Success ? StatusCode(201) : BadRequest(result);
    }
}