using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Host.Controllers;
using Coders_Back.UnitTest.Utils;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Coders_Back.UnitTest.Host.Controllers;

public class AccountControllerTests
{
    [Fact(DisplayName = "Try register a invalid User")]
    public async Task Register_WithInvalidInput_ReturnsBadRequestResult()
    {
        var utils = await AccountControllerTestsUtils.NewUtils(false, true);
        var controller = new AccountController(utils.IdentityServiceMock.Object, utils.EmailServiceProviderMock.Object);

        var result = await controller.Register(utils.RegisterInputs[0]);

        result.Should().BeOfType<BadRequestObjectResult>()
            .Subject.Value.Should().BeOfType<RegisterOutput>()
            .Which.Success.Should().BeFalse();
        
        utils.EmailServiceProviderMock.VerifyNoOtherCalls();
    }
    
    [Fact(DisplayName = "Try to log in with valid credentials")]
    public async Task Login_WithValidCredentials_ReturnsOkResult()
    {
        var utils = await AccountControllerTestsUtils.NewUtils(true, true);
        var controller = new AccountController(utils.IdentityServiceMock.Object, utils.EmailServiceProviderMock.Object);

        var result = await controller.Login(utils.LoginInputs[0]);

        result.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<LoginOutput>()
            .Which.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "Try to log in with invalid credentials")]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedResult()
    {
        var utils = await AccountControllerTestsUtils.NewUtils(true, false);
        var controller = new AccountController(utils.IdentityServiceMock.Object, utils.EmailServiceProviderMock.Object);

        var result = await controller.Login(utils.LoginInputs[0]);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact(DisplayName = "Confirm email with valid userId and token")]
    public async Task ConfirmEmail_WithValidUserIdAndToken_ReturnsContentResult()
    {
        var utils = await AccountControllerTestsUtils.NewUtils(true, false);
        var controller = new AccountController(utils.IdentityServiceMock.Object, utils.EmailServiceProviderMock.Object);
        
        var result = await controller.ConfirmEmail(Guid.NewGuid(), "token");

        result.Should().BeOfType<ContentResult>()
            .Which.Content.Should().NotBeNull();
    }
}