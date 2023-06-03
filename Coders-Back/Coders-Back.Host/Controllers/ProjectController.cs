using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Host.Services;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByUser([FromRoute] Guid id)
    {
        var projects = await _projectService.GetByUser(id);

        var output = projects.Select(project => new ProjectOutput
        {
            Id = project.Id, 
            Name = project.Name, 
            OwnerId = project.OwnerId
        }).ToList();
        
        return Accepted(output);
    }
}