using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Medidores;
using AquaGuard.API.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaGuard.API.Controllers;

[ApiController]
[Route("api/medidores")]
[Authorize]
public class MedidoresController : ControllerBase
{
    private readonly IMedidorService _medidorService;

    public MedidoresController(IMedidorService medidorService)
    {
        _medidorService = medidorService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseViewModel<MedidorResponseViewModel>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        PaginatedResponseViewModel<MedidorResponseViewModel> resultado = await _medidorService.GetAllAsync(page, pageSize);
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedidorResponseViewModel>> GetById(int id)
    {
        MedidorResponseViewModel? medidor = await _medidorService.GetByIdAsync(id);

        if (medidor is null)
            return NotFound(ApiResponseViewModel<MedidorResponseViewModel>.Fail("Medidor não encontrado."));

        return Ok(medidor);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<MedidorResponseViewModel>> Create([FromBody] CreateMedidorViewModel model)
    {
        MedidorResponseViewModel criado = await _medidorService.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<MedidorResponseViewModel>> Update(int id, [FromBody] UpdateMedidorViewModel model)
    {
        MedidorResponseViewModel? atualizado = await _medidorService.UpdateAsync(id, model);

        if (atualizado is null)
            return NotFound(ApiResponseViewModel<MedidorResponseViewModel>.Fail("Medidor não encontrado."));

        return Ok(atualizado);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Deactivate(int id)
    {
        bool sucesso = await _medidorService.DeactivateAsync(id);

        if (!sucesso)
            return NotFound(ApiResponseViewModel<object>.Fail("Medidor não encontrado."));

        return Ok(ApiResponseViewModel<object>.Ok(new { }, "Medidor desativado com sucesso."));
    }
}
