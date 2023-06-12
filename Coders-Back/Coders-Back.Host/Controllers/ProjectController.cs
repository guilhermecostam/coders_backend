using System.Security.Claims;
using Coders_Back.Domain.Extensions;
using Coders_Back.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Coders_Back.Domain.DTOs.Input;
using Microsoft.AspNetCore.Authorization;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _projectService.GetAll());
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var project = await _projectService.GetById(id);
        return project is not null ? Ok(project) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ProjectInput projectInput)
    {
        try
        {
            var userId = User.GetCurrentUserId();
            if (userId is null) return Unauthorized();
            var project = await _projectService.CreateProject(projectInput, userId.Value);
            return Created($"project/{project.Id}", project);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("{projectId:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid projectId, [FromBody] ProjectInput input)
    {
        var isOwner = await _projectService.IsProjectOwner(User.GetCurrentUserId()!.Value, projectId);
        if (!isOwner) return Unauthorized();
        var success = await _projectService.UpdateProject(projectId, input);
        return success ? NoContent() : BadRequest();
    }

    [HttpDelete]
    [Route("{projectId:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid projectId)
    {
        var isOwner = await _projectService.IsProjectOwner(User.GetCurrentUserId()!.Value, projectId);
        if (!isOwner) return Unauthorized();
        var success = await _projectService.DeleteProject(projectId);
        return success ? NoContent() : BadRequest();
    }
    
    [HttpGet]
    [Route("{projectId:guid}/collaborators")]
    public async Task<IActionResult> GetCollaboratorsByProjectId(Guid projectId)
    {
        var collaborators = await _projectService.GetCollaboratorsByProject(projectId);
        return Ok(collaborators);
    }

    [HttpDelete]
    [Route("{projectId:guid}/collaborators/{collaboratorId:guid}")]
    public async Task<IActionResult> DeleteCollaborator(Guid projectId, Guid collaboratorId)
    {
        var isOwner = await _projectService.IsProjectOwner(User.GetCurrentUserId()!.Value, projectId);
        if (!isOwner) return Unauthorized();
        var subClaim = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null) return BadRequest();
        Guid.TryParse(subClaim.Value, out var userId);
        
        await _projectService.DeleteCollaborator(collaboratorId, userId);
        return NoContent();
    }
    
    [HttpGet("{projectId:guid}/requests")]
    public async Task<IActionResult> GetRequests([FromRoute] Guid projectId)
    {
        var isOwner = await _projectService.IsProjectOwner(User.GetCurrentUserId()!.Value, projectId);
        if (!isOwner) return Unauthorized();
        var requestOutputs = await _requestService.GetByProject(projectId);
        return Ok(requestOutputs);
    }
    
    [HttpPost("{projectId:guid}/requests")]
    public async Task<IActionResult> CreateRequest([FromRoute] Guid projectId)
    {
        var userId = User.GetCurrentUserId();
        var result = await _requestService.Create(projectId, userId!.Value);
        return result.Success ? StatusCode(201) : BadRequest(result);
    }
}