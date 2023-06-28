using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Domain.Interfaces;
using Coders_Back.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IEmailServiceProvider _emailServiceProvider;

    public AccountController(IIdentityService identityService, IEmailServiceProvider emailServiceProvider)
    {
        _identityService = identityService;
        _emailServiceProvider = emailServiceProvider;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInput input)
    {
        var (registerResult, emailConfirmationToken) = await _identityService.Register(input);

        if (!registerResult.Success) return BadRequest(registerResult);
        var confirmationUrl = Url.Action(     
            "ConfirmEmail",     
            "Account", 
            new { registerResult.UserId, token = emailConfirmationToken },
            protocol: HttpContext.Request.Scheme );
        
        var emailInput = new SendEmailInput
        {
            Subject = "Confirmação de E-mail Coders!",
            Recipients = new[] { input.Email },
            Body = () => $"<p>Clique no link abaixo para confirmar seu e-mail:</p><p><a href={confirmationUrl}>CLIQUE AQUI</a></p>",
            IsBodyHtml = true
        };
        
        _emailServiceProvider.SendEmail(emailInput);
        registerResult.Message = "Confirme sua conta clicando no link do email que lhe enviamos.";
        
        return Ok(registerResult);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInput input)
    {
        var loginResult = await _identityService.Login(input);
        if (loginResult.Success)
            return Ok(loginResult);
        return Unauthorized(loginResult);
    }

    [HttpGet("confirmEmail")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        await _identityService.ConfirmEmail(userId, token);
        
        const string htmlContent = "<html><body><h1>O seu e-mail foi confirmado com sucesso!</h1></body></html>";

        return Content(htmlContent, "text/html");
    }
}