using AquaGuard.API.Models;

namespace AquaGuard.API.Repositories.Interfaces;

public interface IAlertaRepository : IRepository<Alerta>
{
    Task<(IEnumerable<Alerta> Items, int Total)> GetPagedAsync(int page, int pageSize);
    Task<IEnumerable<Alerta>> GetPendentesAsync();
    Task<int> GetTotalAtivosAsync();
    Task<int> GetTotalVazamentosAsync();
}