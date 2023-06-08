using System.Security.Claims;
using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("project")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _projectService.GetAll());
    }

    [HttpGet]
    [Route("{id:guid}")]
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
}