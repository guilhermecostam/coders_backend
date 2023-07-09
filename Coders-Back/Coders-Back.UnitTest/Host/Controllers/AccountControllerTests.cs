using Coders_Back.Host.Controllers;
using Coders_Back.UnitTest.Host.Utils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Coders_Back.UnitTest.Host.Controllers;

public class AccountControllerTests
{
    [Fact(DisplayName = "Try to register a valid user")]
    public async void TryRegisterNewValidUser()
    {
        var utils = await AccountControllerTestsUtils.NewUtils(true, true);
        
        var controller = new AccountController(utils.IdentityServiceMock.Object, utils.EmailServiceProviderMock.Object);

        var result = await controller.Register(utils.RegisterInputs[0]);
        
        var okResult = result as OkObjectResult;
        var actualConfiguration = okResult.Value.GetType();
        
        //result.
    }
}