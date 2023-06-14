using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Domain.Services;

public class ProjectService : IProjectService
{
    private readonly IGithubApi _githubApi;
    private readonly IRepository<Project> _projects;
    private readonly IRepository<ProjectJoinRequest> _requests;
    private readonly IRepository<ApplicationUser> _users;
    private readonly IRepository<Collaborator> _collaborators;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IRepository<Project> projects, IRepository<ProjectJoinRequest> requests, IRepository<ApplicationUser> users, IUnitOfWork unitOfWork, IRepository<Collaborator> collaborators, IGithubApi githubApi)
    {
        _projects = projects;
        _requests = requests;
        _users = users;
        _unitOfWork = unitOfWork;
        _collaborators = collaborators;
        _githubApi = githubApi;
    }

    public async Task<List<ProjectOutput>> GetAll()
    {
        var usersDbSet = _users.GetDbSet();
        var projectsDbSet = _projects.GetDbSet();
        var query = from project in projectsDbSet
            join user in usersDbSet on project.OwnerId equals user.Id
            select new
            {
                project,
                userGhName = user.GithubProfile
            };
        var projects = await query.ToListAsync();
        return projects.Select( async projectAndName =>
        {
            projectAndName.project.Technologies = await _githubApi.GetTechnologiesByProject(projectAndName.userGhName, GetGhRepositoryNameByUrl(projectAndName.project.GithubUrl));
            return new ProjectOutput(projectAndName.project);
        }).Select(task => task.Result).ToList();
    }

    public async Task<ProjectOutput?> GetById(Guid projectId)
    {
        var project = await _projects.GetById(projectId);
        if (project is null) return null;
        var user = await _users.GetById(project.OwnerId);
        project.Technologies = await _githubApi.GetTechnologiesByProject(user!.GithubProfile, GetGhRepositoryNameByUrl(project.GithubUrl));
        var projectOutput = new ProjectOutput(project);
        return projectOutput;
    }

    public async Task<ProjectOutput> CreateProject(ProjectInput projectInput, Guid ownerId)
    {
        var project = new Project
        {
            Name = projectInput.Name,
            Description = projectInput.Description,
            GithubUrl = projectInput.GithubUrl,
            DiscordUrl = projectInput.DiscordUrl,
            OwnerId = ownerId
        };
        //Fetch Technologies from Github and save them in cache

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

    private static string? GetGhRepositoryNameByUrl(string? repoUrl)
    {
        if (string.IsNullOrEmpty(repoUrl)) return null;
        var url = repoUrl.Split('/');
        return url[^1];
    }
}