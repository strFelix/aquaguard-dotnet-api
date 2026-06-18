using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Auth;
using AquaGuard.API.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AquaGuard.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseViewModel<LoginResponseViewModel>>> Login([FromBody] LoginRequestViewModel model)
    {
        LoginResponseViewModel? resultado = await _authService.LoginAsync(model);

        if (resultado is null)
            return Unauthorized(ApiResponseViewModel<LoginResponseViewModel>.Fail("Email ou senha inválidos."));

        return Ok(ApiResponseViewModel<LoginResponseViewModel>.Ok(resultado, "Login realizado com sucesso."));
    }
}
