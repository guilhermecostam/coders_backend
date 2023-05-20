using Coders_Back.Domain.DTOs.Input;
using Coders_Back.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Coders_Back.Host.Controllers;

[ApiController]
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
        //TODO: Trocar tipo de retorno ?
        return Ok(await _identityService.Register(input));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInput input)
    {
        return Ok(await _identityService.Login(input));
    }
}