using Coders_Back.Domain.DTOs.Output;

namespace Coders_Back.Domain.Interfaces;

public interface IRequestService
{
    Task<ProjectJoinRequestCreateOutput> Create(Guid projectId, Guid userId);
    Task<List<ProjectJoinRequestOutput>> GetByUser(Guid userId);
    Task<List<ProjectJoinRequestOutput>> GetByProject(Guid projectId);
    Task<bool> Reject(Guid requestId, Guid userId);
    Task<bool> Accept(Guid requestId, Guid userId);
}