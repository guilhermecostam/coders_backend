using Coders_Back.Infrastructure.EntityFramework.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("project")]
public class ProjectController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromServices] AppDbContext context
    ){
        var projects = await context
            .Projects
            .AsNoTracking()
            .ToListAsync();

        return Ok(projects);
    }
}