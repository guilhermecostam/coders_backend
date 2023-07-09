using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Interfaces;
using Coders_Back.Infrastructure.Identity.Services;
using Moq;

namespace Coders_Back.UnitTest.Host.Utils;

public class AccountControllerTestsUtils : UnitTestBaseUtils
{
    public Mock<IdentityService> IdentityServiceMock { get; set; }
    public Mock<IEmailServiceProvider> EmailServiceProviderMock { get; set; }
    public List<RegisterInput> RegisterInputs { get; set; }
    
    
    public static async Task<AccountControllerTestsUtils> NewUtils(bool registerShouldBeSuccess, bool loginShouldBeSuccess)
    {
        return new AccountControllerTestsUtils(registerShouldBeSuccess, loginShouldBeSuccess);
    }
    
    private AccountControllerTestsUtils(bool registerShouldBeSuccess, bool loginShouldBeSuccess)
    {
        RegisterInputs = new List<RegisterInput>
        {
            new RegisterInput("AValidEmail@domain.com", "AValidUser", "AValidPassword123.", "AValidPassword123.",
                "A Valid Name For User")
        };

        IdentityServiceMock = new Mock<IdentityService>();
        IdentityServiceMock.Setup(m => m.Register(It.IsAny<RegisterInput>())).ReturnsAsync((new RegisterOutput
        {
            Success = registerShouldBeSuccess,
            Errors = registerShouldBeSuccess ? new List<string>{"Error"} : null,
            UserId = Guid.NewGuid(),
            Message = null
        }, "token"));
        
        EmailServiceProviderMock = new Mock<IEmailServiceProvider>();
        EmailServiceProviderMock.Setup(m => m.SendEmail(It.IsAny<SendEmailInput>()));

    }
}