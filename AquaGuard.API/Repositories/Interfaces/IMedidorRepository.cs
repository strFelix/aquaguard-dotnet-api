using AquaGuard.API.Models;

namespace AquaGuard.API.Repositories.Interfaces;

public interface IMedidorRepository : IRepository<Medidor>
{
    Task<Medidor?> GetByCodigoAsync(string codigo);
    Task<(IEnumerable<Medidor> Items, int Total)> GetPagedAsync(int page, int pageSize);
}