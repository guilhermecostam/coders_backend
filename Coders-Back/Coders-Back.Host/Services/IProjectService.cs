using Coders_Back.Domain.Entities;

namespace Coders_Back.Host.Services;

public interface IProjectService
{
    Task<List<Project>> GetByUser(Guid id);
}