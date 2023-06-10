using System.Security.Claims;
using Coders_Back.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Coders_Back.Domain.DTOs.Input;

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

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] ProjectInput projectInput
    ){
        try
        {
            var project = await _projectService.Create(projectInput);
            return Created($"project/{project.Id}", project);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ProjectInput input)
    {
        var success = await _projectService.Update(id);
        return success ? NoContent() : BadRequest();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var success = await _projectService.Delete(id);
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