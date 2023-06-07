using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;

namespace Coders_Back.Domain.Interfaces;

public interface IProjectService
{
    Task<List<ProjectOutput>> GetAll();
    Task<ProjectOutput?> GetById(Guid projectId);
}