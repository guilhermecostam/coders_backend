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
    private readonly IUnitOfWork _unitOfWork;

    public RequestService(IRepository<Project> projects, IRepository<ProjectJoinRequest> requests, IUnitOfWork unitOfWork, IRepository<Collaborator> collaborators)
    {
        _projects = projects;
        _requests = requests;
        _unitOfWork = unitOfWork;
        _collaborators = collaborators;
    }

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

    public async Task<List<ProjectJoinRequestOutput>> GetByProject(Guid projectId)
    {
        var requestDbSet = _requests.GetDbSet();
        var requests = requestDbSet.Where(r =>
            r.ProjectId == projectId && r.Status == RequestStatus.Open).Select(r => new ProjectJoinRequestOutput(r));
        return await requests.ToListAsync();
    }

    public async Task<bool> Reject(Guid requestId, Guid userId)
    {
        var request = await _requests.GetById(requestId);
        if (request is null) return false;
        
        if (!await CheckUserIsOwner(userId, request)) return false;
        
        request.Status = RequestStatus.Rejected;
        _requests.Update(request);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Accept(Guid requestId, Guid userId)
    {
        var request = await _requests.GetById(requestId);
        if (request is null) return false;

        if (!await CheckUserIsOwner(userId, request)) return false;
            
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
    
    private async Task<bool> CheckUserIsOwner(Guid userId, ProjectJoinRequest request)
    {
        //TODO: create a new extension for it
        var project = await _projects.GetById(request.ProjectId);
        if (project is null) return false;
        return project.OwnerId == userId;
    }
}