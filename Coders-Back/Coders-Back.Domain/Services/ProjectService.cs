using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;

namespace Coders_Back.Domain.Services;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projects;
    private readonly IRepository<ProjectJoinRequest> _requests;
    private readonly IRepository<ApplicationUser> _users;
    private readonly IRepository<Collaborator> _collaborators;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IRepository<Project> projects, IRepository<ProjectJoinRequest> requests, IRepository<ApplicationUser> users, IUnitOfWork unitOfWork, IRepository<Collaborator> collaborators)
    {
        _projects = projects;
        _requests = requests;
        _users = users;
        _unitOfWork = unitOfWork;
        _collaborators = collaborators;
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

    public async Task<ProjectOutput> CreateProject(ProjectInput projectInput, Guid ownerId)
    {
        var project = new Project{
            Name = projectInput.Name,
            Description = projectInput.Description,
            GithubUrl = projectInput.GithubUrl,
            DiscordUrl = projectInput.DiscordUrl,
            OwnerId = ownerId
        };

        await _projects.Insert(project);
        await _collaborators.Insert(new Collaborator
        {
            UserId = ownerId,
            ProjectId = project.Id
        });

        await _unitOfWork.SaveChangesAsync();
        
        return new ProjectOutput(project);
    }

    public async Task<bool> UpdateProject(Guid id, ProjectInput input)
    {
        var project = await _projects.GetById(id);
        if (project is null) return false;

        project.GithubUrl = input.GithubUrl;
        project.Description = input.Description;
        project.DiscordUrl = input.DiscordUrl;
        project.Name = input.Name;
            
        _projects.Update(project);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteProject(Guid projectId)
    {
        var project = await _projects.GetById(projectId);
        if (project is null) return false;
        await _projects.Delete(projectId);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<List<CollaboratorOutput>?> GetCollaboratorsByProject(Guid projectId)
    {
        var project = await _projects.GetById(projectId);

        if (project is null)
        {
            return null;
        }

        var collaborators = await _collaborators.GetAll();
        var projectCollaborators = collaborators
            .Where(c => c.ProjectId == projectId)
            .Select(c => new CollaboratorOutput(c))
            .ToList();

        return projectCollaborators;
    }

    public async Task DeleteCollaborator(Guid collaboratorId, Guid userId)
    {
        //TODO : block delete owner collaborator !! 
        var collaborator = await _collaborators.GetById(collaboratorId);
        if (collaborator != null) return;
        
        var project = await _projects.GetById(collaborator!.ProjectId);
        if (project != null) return;

        if (project!.OwnerId == userId)
        {
            _collaborators.Delete(collaborator);
            await _unitOfWork.SaveChangesAsync();
        }
    }
    
    public async Task<bool> IsProjectOwner(Guid userId, Guid projectId)
    {
        var project = await _projects.GetById(projectId);
        if (project is null) return false;
        return project.OwnerId == userId;
    }
}