using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.DTOs.Output;
using Coders_Back.Domain.Enums;
using Coders_Back.Domain.Interfaces;
using Coders_Back.Infrastructure.Identity.Services;
using Moq;

namespace Coders_Back.UnitTest.Utils;

public class AccountControllerTestsUtils : UnitTestBaseUtils
{
    public Mock<IIdentityService> IdentityServiceMock { get; set; }
    public Mock<IEmailServiceProvider> EmailServiceProviderMock { get; set; }
    public List<RegisterInput> RegisterInputs { get; set; }
    public List<LoginInput> LoginInputs { get; set; }


    public static async Task<AccountControllerTestsUtils> NewUtils(bool registerShouldBeSuccess, bool loginShouldBeSuccess)
    {
        return new AccountControllerTestsUtils(registerShouldBeSuccess, loginShouldBeSuccess);
    }
    
    private AccountControllerTestsUtils(bool registerShouldBeSuccess, bool loginShouldBeSuccess)
    {
        LoginInputs = new List<LoginInput>
        {
            new("someIdentifier", "SomePass"),
        };
        
        RegisterInputs = new List<RegisterInput>
        {
            new RegisterInput("AValidEmail@domain.com", "AValidUser", "AValidPassword123.", "AValidPassword123.",
                "A Valid Name For User")
        };

        IdentityServiceMock = new Mock<IIdentityService>();
        IdentityServiceMock.Setup(m => m.Register(It.IsAny<RegisterInput>())).ReturnsAsync((new RegisterOutput
        {
            Success = registerShouldBeSuccess,
            Errors = registerShouldBeSuccess ? new List<string>{"Error"} : null,
            UserId = Guid.NewGuid(),
            Message = null
        }, "token"));
        
        EmailServiceProviderMock = new Mock<IEmailServiceProvider>();
        EmailServiceProviderMock.Setup(m => m.SendEmail(It.IsAny<SendEmailInput>()));
        
        IdentityServiceMock.Setup(s => s.ConfirmEmail(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        if (loginShouldBeSuccess)
        {
            IdentityServiceMock.Setup(s => s.Login(LoginInputs![0]))
                .ReturnsAsync(new LoginOutput { Success = true });
        }
        else
        {
            IdentityServiceMock.Setup(s => s.Login(LoginInputs![0]))
                .ReturnsAsync(new LoginOutput
                {
                    Success = false,
                    LoginError = LoginErrorsOutput.InvalidUsernameOrPassword
                    
                });
        }
    }

    
}