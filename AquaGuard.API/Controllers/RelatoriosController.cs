using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Relatorios;
using AquaGuard.API.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaGuard.API.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    [HttpGet("mensal")]
    public async Task<ActionResult<RelatorioMensalViewModel>> GetMensal(
        [FromQuery] int ano,
        [FromQuery] int mes)
    {
        RelatorioMensalViewModel resultado = await _relatorioService.GetRelatorioMensalAsync(ano, mes);
        return Ok(resultado);
    }

    [HttpGet("medidor/{id:int}")]
    public async Task<ActionResult<RelatorioMedidorViewModel>> GetPorMedidor(int id)
    {
        RelatorioMedidorViewModel? resultado = await _relatorioService.GetRelatorioMedidorAsync(id);

        if (resultado is null)
            return NotFound(ApiResponseViewModel<RelatorioMedidorViewModel>.Fail("Medidor não encontrado."));

        return Ok(resultado);
    }
}
