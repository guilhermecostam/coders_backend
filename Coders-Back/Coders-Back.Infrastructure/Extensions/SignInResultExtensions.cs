using Coders_Back.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Coders_Back.Infrastructure.Extensions;

public static class SignInResultExtensions
{

    public static LoginErrorsOutput? GetSignInResultErrors(this SignInResult result)
    {
        if (result.Succeeded) return null;
        
        if (result.IsLockedOut)
            return LoginErrorsOutput.AccountBlocked;
        if (result.IsNotAllowed)
            return LoginErrorsOutput.AccountNotAllowed;
            
        return result.RequiresTwoFactor ? LoginErrorsOutput.TwoFactorAuthenticationRequired : LoginErrorsOutput.InvalidUsernameOrPassword;
    }
    
}