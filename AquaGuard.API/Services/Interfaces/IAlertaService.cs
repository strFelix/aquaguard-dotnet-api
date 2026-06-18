using AquaGuard.API.ViewModels.Alertas;
using AquaGuard.API.ViewModels.Shared;

namespace AquaGuard.API.Services.Interfaces;

public interface IAlertaService
{
    Task<PaginatedResponseViewModel<AlertaResponseViewModel>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<AlertaResponseViewModel>> GetPendentesAsync();
    Task<AlertaResponseViewModel?> ResolverAsync(int id);
}
