using System.Security.Claims;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.Entities;
using Coders_Back.Infrastructure.Identity.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace Coders_Back.UnitTest.Infrastructure;

public class IdentityServiceTestsUtils
{
    public RegisterInput RegisterInput { get; set; }
    public LoginInput LoginInput { get; set; }
    public ApplicationUser User { get; set; }
    public Mock<UserManager<ApplicationUser>> UserManagerMock  { get; set; }
    public Mock<SignInManager<ApplicationUser>> SinInManagerMock { get; set; }
    public Mock<IOptions<JwtOptions>>  JwtOptionsMock { get; set; }
    
    public static IdentityServiceTestsUtils NewUtils(bool createUserMustFail = false)
    {
        return new IdentityServiceTestsUtils(createUserMustFail);
    }

    private IdentityServiceTestsUtils(bool createUserMustFail)
    {
        RegisterInput = new RegisterInput("some@email.com", "An_amazing_person", "AInvalidPassword",
            "AInvalidPassword", "A Amazing Peron");
        
        User = new ApplicationUser
        {
            UserName = "An_amazing_person",
            Email = "some@email.com",
            EmailConfirmed = true
        };

        LoginInput = new LoginInput(RegisterInput.Email, RegisterInput.Password);
        
        UserManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        if (createUserMustFail)
            UserManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "Error", Description = "An Error" }));
        else
            UserManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

        UserManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ApplicationUser>(null!));

        UserManagerMock.Setup(m => m.GetClaimsAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<Claim>());
        UserManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());
        UserManagerMock.Setup(m => m.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(new IdentityResult());
        
        SinInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            UserManagerMock.Object, Mock.Of<IHttpContextAccessor>(), 
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), 
            null, null, null, null);
        
        SinInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);
        
        JwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        
        var jwtOptions = new JwtOptions
        {
            Issuer = "UnitTests",
            Audience = "UnitTests",
            SigningCredentials = null,
            Expiration = 3600
        };
        
        JwtOptionsMock.Setup(o => o.Value).Returns(jwtOptions);
    }
}