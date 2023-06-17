using Coders_Back.Domain.DataAbstractions;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Enums;
using Coders_Back.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Coders_Back.Domain.Services;

public class RequestService : IRequestService
{
    private readonly IRepository<Project> _projects;
    private readonly IRepository<ProjectJoinRequest> _requests;
    private readonly IRepository<Collaborator> _collaborators;
    private readonly IProjectService _projectService;
    private readonly IUnitOfWork _unitOfWork;

    public RequestService(IRepository<Project> projects, IRepository<ProjectJoinRequest> requests, IUnitOfWork unitOfWork, IRepository<Collaborator> collaborators, IProjectService projectService)
    {
        _projects = projects;
        _requests = requests;
        _unitOfWork = unitOfWork;
        _collaborators = collaborators;
        _projectService = projectService;
    }

    //TODO: refactor this passing all validation logic to specify method
    public async Task<ProjectJoinRequestCreateOutput> Create(Guid projectId, Guid userId)
    {
        var project = await _projects.GetById(projectId);
        if(project is null) return new ProjectJoinRequestCreateOutput
            {
                Success = false,
                Error = RequestCreateOutputError.ProjectNotFound
            };
        
        var requestDbSet = _requests.GetDbSet();
        
        //verificar se irá considerar os deletados, caso sim, filtrar manualmente para que possamos criar uma mesma request caso queira
        var alreadyExists = requestDbSet.Any(r => r.ProjectId == projectId && r.UserId == userId);
        if (alreadyExists) return new ProjectJoinRequestCreateOutput
            {
                Success = false,
                Error = RequestCreateOutputError.RequestAlreadyExists
            };

        var collaboratorDbSet = _collaborators.GetDbSet();
        var collaboratorAlreadyExists = collaboratorDbSet.Any(c =>
            c.ProjectId == projectId && c.UserId == userId);
        if (collaboratorAlreadyExists) return new ProjectJoinRequestCreateOutput
            {
                Success = false,
                Error = RequestCreateOutputError.CollaboratorAlreadyExists
            };
        
        await _requests.Insert(new ProjectJoinRequest
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            UserId = userId,
            Status = RequestStatus.Open
        });
        
        await _unitOfWork.SaveChangesAsync();
        
        return new ProjectJoinRequestCreateOutput
        {
            Success = true,
            Error = null
        };
    }

    public async Task<List<ProjectJoinRequestOutput>> GetByUser(Guid userId)
    {
        var requestDbSet = _requests.GetDbSet();
        var requests = requestDbSet.Where(r =>
            r.UserId == userId && r.Status == RequestStatus.Open).Select(r => new ProjectJoinRequestOutput(r));
        return await requests.ToListAsync();
    }

    public async Task<List<ProjectJoinRequestOutput>> GetPendingByUser(Guid userId)
    {
        var requestDbSet = _requests.GetDbSet();
        var projectDbSet = _projects.GetDbSet();

        var userProjects = await projectDbSet.Where(p => p.OwnerId == userId).Select(p => p.Id).ToListAsync();
        var pendingRequests = await requestDbSet.Where(r =>
                userProjects.Contains(r.ProjectId) && r.Status == RequestStatus.Open)
            .Select(r => new ProjectJoinRequestOutput(r)).ToListAsync();
        return pendingRequests;
    }

    public async Task<List<ProjectJoinRequestOutput>> GetPendingByProject(Guid projectId)
    {
        var requestDbSet = _requests.GetDbSet();
        var requests = requestDbSet.Where(r =>
            r.ProjectId == projectId && r.Status == RequestStatus.Open).Select(r => new ProjectJoinRequestOutput(r));
        return await requests.ToListAsync();
    }

    public async Task<bool> Reject(Guid requestId, Guid currentUserId)
    {
        var request = await _requests.GetById(requestId);
        if (request is null) return false;
        
        var isOwner = await _projectService.IsProjectOwner(currentUserId, request.ProjectId);
        if (!isOwner) return false;
        
        request.Status = RequestStatus.Rejected;
        _requests.Update(request);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Accept(Guid requestId, Guid currentUserId)
    {
        var request = await _requests.GetById(requestId);
        if (request is null) return false;

        var isOwner = await _projectService.IsProjectOwner(currentUserId, request.ProjectId);
        if (!isOwner) return false;
            
        request.Status = RequestStatus.Accepted;
        _requests.Update(request);
        //TODO: use create method from CollaboratorsService
        await _collaborators.Insert(new Collaborator
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
            UserId = request.UserId
        });
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}