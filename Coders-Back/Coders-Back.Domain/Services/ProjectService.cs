using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;

namespace Coders_Back.Domain.Services;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projects;

    public ProjectService(IRepository<Project> projects)
    {
        _projects = projects;
    }

    public async Task<List<ProjectOutput>> GetAll()
    {
        var projects = await _projects.GetAll();
        return projects.Select(p => new ProjectOutput(p)).ToList();
    }

    public async Task<ProjectOutput?> GetById(Guid projectId)
    {
        var project = await _projects.GetById(projectId);
        return project is null ? null : new ProjectOutput(project);
    }
}