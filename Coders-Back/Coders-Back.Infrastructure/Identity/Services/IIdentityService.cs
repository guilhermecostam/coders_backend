using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;

namespace Coders_Back.Infrastructure.Identity.Services;

public interface IIdentityService
{
    Task ConfirmEmail(Guid userId, string token);
    Task<RegisterOutput> Register(RegisterInput input);
    Task<LoginOutput> Login(LoginInput input);
}