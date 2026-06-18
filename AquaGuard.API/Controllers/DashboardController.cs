using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Relatorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquaGuard.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardViewModel>> Get()
    {
        DashboardViewModel resultado = await _dashboardService.GetDashboardAsync();
        return Ok(resultado);
    }
}
