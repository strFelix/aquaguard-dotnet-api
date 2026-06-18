using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Alertas;
using AquaGuard.API.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaGuard.API.Controllers;

[ApiController]
[Route("api/alertas")]
[Authorize]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _alertaService;

    public AlertasController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseViewModel<AlertaResponseViewModel>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        PaginatedResponseViewModel<AlertaResponseViewModel> resultado = await _alertaService.GetAllAsync(page, pageSize);
        return Ok(resultado);
    }

    [HttpGet("pendentes")]
    public async Task<ActionResult<IEnumerable<AlertaResponseViewModel>>> GetPendentes()
    {
        IEnumerable<AlertaResponseViewModel> resultado = await _alertaService.GetPendentesAsync();
        return Ok(resultado);
    }

    [HttpPut("{id:int}/resolver")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<AlertaResponseViewModel>> Resolver(int id)
    {
        AlertaResponseViewModel? resolvido = await _alertaService.ResolverAsync(id);

        if (resolvido is null)
            return NotFound(ApiResponseViewModel<AlertaResponseViewModel>.Fail("Alerta não encontrado."));

        return Ok(resolvido);
    }
}
