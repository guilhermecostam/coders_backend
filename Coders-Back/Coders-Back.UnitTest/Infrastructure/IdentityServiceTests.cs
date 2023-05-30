using Coders_Back.Domain.Entities;
using Coders_Back.Domain.Enums;
using Coders_Back.Infrastructure.Identity.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace Coders_Back.UnitTest.Infrastructure;

public class IdentityServiceTests
{
    [Fact(DisplayName = "Try to register user with invalid fields")]
    public async void RegisterInvalidUser()
    {
        var utils = IdentityServiceTestsUtils.NewUtils(true);
        var identityService = new IdentityService(utils.SinInManagerMock.Object, utils.UserManagerMock.Object,  utils.JwtOptionsMock.Object);
       
        var result = await identityService.Register(utils.RegisterInput);

        //Assert
        result.Errors!.Count.Should().BeGreaterThan(0);
        result.Success.Should().BeFalse();

        utils.UserManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);

        utils.UserManagerMock.Verify(m => 
            m.SetLockoutEnabledAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>()), Times.Never);
    }
    
    [Fact(DisplayName = "Try to register a valid new user")]
    public async void RegisterValidUser()
    {
        var utils = IdentityServiceTestsUtils.NewUtils();
        var identityService = new IdentityService(utils.SinInManagerMock.Object, utils.UserManagerMock.Object,  utils.JwtOptionsMock.Object);
       
        var result = await identityService.Register(utils.RegisterInput);
        
        //Assert
        result.Success.Should().BeTrue();
        result.Errors.Should().BeNull();
        
        utils.UserManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        utils.UserManagerMock.Verify(m => m.SetLockoutEnabledAsync(It.IsAny<ApplicationUser>(), It.Is<bool>(e => e == false)));
    }
    
    [Fact(DisplayName = "Try to login with invalid User")]
    public async void LoginWithInvalidUser()
    {
        var utils = IdentityServiceTestsUtils.NewUtils();
        var identityService = new IdentityService(utils.SinInManagerMock.Object, utils.UserManagerMock.Object,  utils.JwtOptionsMock.Object);
       
        var result = await identityService.Login(utils.LoginInput);
        
        //Assert
        result.Success.Should().BeFalse();
        result.Token.Should().BeNull();
        result.LoginError.Should().Be(LoginErrorsOutput.InvalidUsernameOrPassword);
        
        utils.UserManagerMock.Verify(m => 
            m.FindByEmailAsync(It.IsAny<string>()), Times.Once);
    }
    
    [Fact(DisplayName = "Try to login with valid User")]
    public async void LoginWithValidUser()
    {
        var utils = IdentityServiceTestsUtils.NewUtils();
        var identityService = new IdentityService(utils.SinInManagerMock.Object, utils.UserManagerMock.Object,  utils.JwtOptionsMock.Object);
        
        utils.UserManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(utils.User);
        
        var result = await identityService.Login(utils.LoginInput);
        
        //Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        result.LoginError.Should().BeNull();
        
        utils.UserManagerMock.Verify(m => 
            m.FindByEmailAsync(It.IsAny<string>()), Times.Exactly(1));
        utils.SinInManagerMock.Verify(m => 
            m.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
    }
}