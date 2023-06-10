using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;

namespace Coders_Back.Domain.Interfaces;

public interface IProjectService
{
    Task<List<ProjectOutput>> GetAll();
    Task<ProjectOutput?> GetById(Guid projectId);
    Task<ProjectOutput> Create(ProjectInput projectInput);
    Task<bool> Update(Guid projectId);
    Task<bool> Delete(Guid projectId);
}