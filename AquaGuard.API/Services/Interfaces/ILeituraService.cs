using AquaGuard.API.ViewModels.Leituras;
using AquaGuard.API.ViewModels.Shared;

namespace AquaGuard.API.Services.Interfaces;

public interface ILeituraService
{
    Task<PaginatedResponseViewModel<LeituraResponseViewModel>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<LeituraResponseViewModel>> GetByMedidorAsync(int medidorId);
    Task<IEnumerable<LeituraResponseViewModel>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
    Task<LeituraResponseViewModel> CreateAsync(CreateLeituraViewModel model, int usuarioId);
}
