using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;

namespace Coders_Back.Domain.Interfaces;

public interface IProjectService
{
    Task<List<ProjectOutput>> GetAll();
    Task<ProjectOutput?> GetById(Guid projectId);
    Task<List<CollaboratorOutput>?> GetCollaboratorsByProject(Guid projectId);
    Task DeleteCollaborator(Guid collaboratorId, Guid userId);
    Task<ProjectOutput> CreateProject(ProjectInput projectInput, Guid ownerId);
    Task<bool> UpdateProject(Guid id, ProjectInput input);
    Task<bool> DeleteProject(Guid projectId);
    Task<bool> IsProjectOwner(Guid userId, Guid projectId);
}