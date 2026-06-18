using AquaGuard.API.ViewModels.Relatorios;

namespace AquaGuard.API.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}
