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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterInput input)
    {
        //don't use this because [ApiController] attribute already does that
        // if (!ModelState.IsValid)
        //     return BadRequest();
        
        var registerResult = await _identityService.Register(input);
        
        if (registerResult.Success)
            return Ok(registerResult);

        return BadRequest(registerResult);
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