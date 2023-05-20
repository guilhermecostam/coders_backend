using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Enums;
using Coders_Back.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly JwtOptions _jwtOptions;

    public IdentityService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        //_jwtOptions = jwtOptions;
    }

    public async Task<RegisterOutput> Register(RegisterInput input)
    {
        var user = new ApplicationUser
        {
            Name = input.Name,
            UserName = input.Email,
            Email = input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
            return new RegisterOutput { Success = false, Errors = result.Errors.Select(error => error.Description).ToList() };
        
        await _userManager.SetLockoutEnabledAsync(user, false);
        return new RegisterOutput { Success = result.Succeeded, };
    }

    public async Task<LoginOutput> Login(LoginInput input)
    {
        var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, false, false);
        return new LoginOutput
        {
            Success = result.Succeeded,
            Token = result.Succeeded ? await GetToken(input.Email) : null,
            Status = result.Succeeded ? LoginStatusOutput.Success : LoginStatusOutput.Failed
        };
    }

    private async Task<string> GetToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return "a";
    }
    
    private async Task<string> GetClaimsAndRoles(ApplicationUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Guid.NewGuid().ToString();

    }
}