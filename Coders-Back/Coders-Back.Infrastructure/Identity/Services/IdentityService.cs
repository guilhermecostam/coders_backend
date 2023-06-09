using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Enums;
using Coders_Back.Infrastructure.Extensions;
using Coders_Back.Infrastructure.Identity.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Coders_Back.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public IdentityService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtOptions = jwtOptions;
    }

    public async Task ConfirmEmail(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        await _userManager.ConfirmEmailAsync(user, token);
    }
    public async Task<(RegisterOutput, string?)> Register(RegisterInput input)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = input.UserName,
            Email = input.Email,
            EmailConfirmed = false,
            Name = input.Name
        };

        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
            return (new RegisterOutput
                { Success = false, Errors = result.Errors.Select(error => error.Description).ToList() }, null);

        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _userManager.SetLockoutEnabledAsync(user, false);
        return (new RegisterOutput { Success = result.Succeeded, UserId = user.Id }, emailConfirmationToken);
    }

    public async Task<LoginOutput> Login(LoginInput input)
    {
        ApplicationUser user;

        if (new EmailAddressAttribute().IsValid(input.Identifier))
            user = await _userManager.FindByEmailAsync(input.Identifier);
        else
            user = await _userManager.FindByNameAsync(input.Identifier);

        if (user is null)
        {
            return new LoginOutput
            {
                Success = false,
                Token = null,
                LoginError = LoginErrorsOutput.InvalidUsernameOrPassword    
            };
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return new LoginOutput
            {
                Success = false,
                LoginError = LoginErrorsOutput.EmailNotConfirmed
            };
        }
        
        var result = await _signInManager.PasswordSignInAsync(user, input.Password, false, false);
        return new LoginOutput
        {
            Success = result.Succeeded,
            Token = result.Succeeded ? await GetToken(user) : null,
            LoginError = result.GetSignInResultErrors()
        };
    }   

    private async Task<string> GetToken(ApplicationUser user)
    {
        var claims = await GetClaims(user);
        var now = DateTime.Now;
        var expirationTime = now.AddSeconds(_jwtOptions.Value.Expiration); 
        
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Value.Issuer,
            audience: _jwtOptions.Value.Audience,
            claims: claims,
            notBefore: now,
            expires: expirationTime,
            signingCredentials: _jwtOptions.Value.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    private async Task<IList<Claim>> GetClaims(ApplicationUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var dateTimeNow = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, dateTimeNow));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, dateTimeNow));

        foreach (var role in roles)
            claims.Add(new Claim("role", role));

        return claims;
    }
}