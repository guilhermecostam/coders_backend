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
    public ApplicationUser Users { get; set; }
    public Mock<UserManager<ApplicationUser>> UserManagerMock  { get; set; }
    public Mock<SignInManager<ApplicationUser>> SinInManagerMock { get; set; }
    public Mock<IOptions<JwtOptions>>  JwtOptionsMock { get; set; }
    
    public static IdentityServiceTestsUtils NewUtils()
    {
        return new IdentityServiceTestsUtils();
    }

    private IdentityServiceTestsUtils()
    {
        RegisterInput = new RegisterInput("amazingemail.something", "An amazing person", "AInvalidPassword",
            "AInvalidPassword");
        
        Users = new ApplicationUser
        {
            UserName = "An amazing person",
            Email = "amazingemail.something",
            EmailConfirmed = true
        }; 
        
        UserManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        
        UserManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidEmailFormat", Description = "Email is in an invalid format." }));
        
        SinInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            UserManagerMock.Object, Mock.Of<IHttpContextAccessor>(), 
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), 
            null, null, null, null);
        
        JwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        
        var jwtOptions = new JwtOptions
        {
            Issuer = "UnitTests",
            Audience = "UnitTests",
            SigningCredentials = null,
            Expiration = 0
        };
        
        JwtOptionsMock.Setup(o => o.Value).Returns(jwtOptions);
    }
}