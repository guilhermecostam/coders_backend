using System.Net;
using System.Net.Mail;
using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AccountController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        await _identityService.ConfirmEmail(userId, token);
        
        const string htmlContent = "<html><body><h1>O seu e-mail foi confirmado com sucesso!</h1></body></html>";

        return Content(htmlContent, "text/html");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInput input)
    {
        var registerResult = await _identityService.Register(input);

        if (!registerResult.Success) return BadRequest(registerResult);
        var confirmationUrl = Url.Action(     
            "ConfirmEmail",     
            "Account", 
            new { registerResult.UserId, token = registerResult.ConfirmationEmailToken },
            protocol: HttpContext.Request.Scheme );
        
        var message = new MailMessage();
        message.From = new MailAddress("codersproject@outlook.com");
        message.To.Add(input.Email);
        message.Subject = "Confirmação de E-mail Coders!";
        
        var body = $"<p>Clique no link abaixo para confirmar seu e-mail:</p>" +
                      $"<p><a href={confirmationUrl}>CLIQUE AQUI</a></p>";
    
        message.Body = body;
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.office365.com", 587);
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential("null", "null"); // Ocultamos para não aparecer informações confidenciais do projeto
        smtpClient.EnableSsl = true;
        
        smtpClient.Send(message);
            
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
}