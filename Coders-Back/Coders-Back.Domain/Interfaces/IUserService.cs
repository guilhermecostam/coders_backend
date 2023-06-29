using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;

namespace Coders_Back.Domain.Interfaces;

public interface IUserService
{
    Task<bool> UpdateUser(UserInput input);
    Task<List<ProjectOutput>?> GetProjectsByUser(Guid id);
}