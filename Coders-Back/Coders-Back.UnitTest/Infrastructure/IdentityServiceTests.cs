using Coders_Back.Domain.Entities;
using Coders_Back.Infrastructure.Identity.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Coders_Back.UnitTest.Infrastructure;

public class IdentityServiceTests
{
    [Fact(DisplayName = "Try to register user with invalid email")]
    public async void RegisterInvalidEmail()
    {
        var utils = IdentityServiceTestsUtils.NewUtils();
        var identityService = new IdentityService(utils.SinInManagerMock.Object, utils.UserManagerMock.Object,  utils.JwtOptionsMock.Object);
       
        var result = await identityService.Register(utils.RegisterInput);

        //Assert
        result.Errors!.Count.Should().BeGreaterThan(0);
        result.Success.Should().BeFalse();

        utils.UserManagerMock.Verify(m =>
            m.CreateAsync(It.Is<ApplicationUser>(u =>
                    u.UserName!.Equals(utils.Users.UserName) && u.Email.Equals(utils.Users.Email) && u.UserName.Equals(utils.Users.UserName) && u.TwoFactorEnabled == utils.Users.TwoFactorEnabled),
                It.Is<string>(p => p.Equals(utils.RegisterInput.Password))), Times.Once);
        
        utils.UserManagerMock.Verify(m => 
            m.SetLockoutEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()), Times.Never);
    }
    
    [Fact(DisplayName = "Try to register a valid new user")]
    public async void RegisterValidUser()
    {
        //TODO: 
    }
}