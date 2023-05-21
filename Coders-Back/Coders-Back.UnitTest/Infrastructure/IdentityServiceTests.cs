using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.Entities;
using Coders_Back.Infrastructure.Identity.Configurations;
using Coders_Back.Infrastructure.Identity.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Coders_Back.UnitTest.Infrastructure;

public class IdentityServiceTests
{
    [Fact(DisplayName = "Try to register user with invalid email")]
    public async void RegisterInvalidEmail()
    {
        //TODO: performe all arranges and setups in Utils Class
        //Arrange
        var registerInput = new RegisterInput("amazingemail.something", "An amazing person", "AInvalidPassword",
            "AInvalidPassword");
        
        var user = new ApplicationUser
        {
            UserName = registerInput.UserName,
            Email = registerInput.Email,
            EmailConfirmed = true
        }; 
        
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
        
        userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "InvalidEmailFormat", Description = "Email is in an invalid format." }));

        var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), 
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), 
            null, null, null, null);

        var jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        var jwtOptions = new JwtOptions
        {
            Issuer = "UnitTests",
            Audience = "UnitTests",
            SigningCredentials = null,
            Expiration = 0
        };
        jwtOptionsMock.Setup(o => o.Value).Returns(jwtOptions);
        
        var identityService = new IdentityService(signInManagerMock.Object, userManagerMock.Object,  jwtOptionsMock.Object);
        
        //Act
        var result = await identityService.Register(registerInput);

        //Assert
        result.Errors!.Count.Should().BeGreaterThan(0);
        result.Success.Should().BeFalse();

        userManagerMock.Verify(m =>
            m.CreateAsync(It.Is<ApplicationUser>(u =>
                    u.UserName!.Equals(user.UserName) && u.Email.Equals(user.Email) && u.UserName.Equals(user.UserName) && u.TwoFactorEnabled == user.TwoFactorEnabled),
                It.Is<string>(p => p.Equals(registerInput.Password))), Times.Once);
        
        userManagerMock.Verify(m => 
            m.SetLockoutEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()), Times.Never);
    }
    
    [Fact(DisplayName = "Try to register a valid new user")]
    public async void RegisterValidUser()
    {
        //TODO: 
    }
}