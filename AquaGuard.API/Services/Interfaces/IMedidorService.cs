using AquaGuard.API.ViewModels.Medidores;
using AquaGuard.API.ViewModels.Shared;

namespace AquaGuard.API.Services.Interfaces;

public interface IMedidorService
{
    Task<PaginatedResponseViewModel<MedidorResponseViewModel>> GetAllAsync(int page, int pageSize);
    Task<MedidorResponseViewModel?> GetByIdAsync(int id);
    Task<MedidorResponseViewModel> CreateAsync(CreateMedidorViewModel model);
    Task<MedidorResponseViewModel?> UpdateAsync(int id, UpdateMedidorViewModel model);
    Task<bool> DeactivateAsync(int id);
}
