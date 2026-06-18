using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Leituras;
using AquaGuard.API.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AquaGuard.API.Controllers;

[ApiController]
[Route("api/leituras")]
[Authorize]
public class LeiturasController : ControllerBase
{
    private readonly ILeituraService _leituraService;

    public LeiturasController(ILeituraService leituraService)
    {
        _leituraService = leituraService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseViewModel<LeituraResponseViewModel>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        PaginatedResponseViewModel<LeituraResponseViewModel> resultado = await _leituraService.GetAllAsync(page, pageSize);
        return Ok(resultado);
    }

    [HttpGet("periodo")]
    public async Task<ActionResult<IEnumerable<LeituraResponseViewModel>>> GetByPeriodo(
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim)
    {
        IEnumerable<LeituraResponseViewModel> resultado = await _leituraService.GetByPeriodoAsync(inicio, fim);
        return Ok(resultado);
    }

    [HttpGet("medidor/{id:int}")]
    public async Task<ActionResult<IEnumerable<LeituraResponseViewModel>>> GetByMedidor(int id)
    {
        IEnumerable<LeituraResponseViewModel> resultado = await _leituraService.GetByMedidorAsync(id);
        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<LeituraResponseViewModel>> Create([FromBody] CreateLeituraViewModel model)
    {
        string? usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out int usuarioId))
            return Unauthorized(ApiResponseViewModel<LeituraResponseViewModel>.Fail("Usuário não identificado."));

        LeituraResponseViewModel criada = await _leituraService.CreateAsync(model, usuarioId);
        return Ok(criada);
    }
}
