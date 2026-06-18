using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.API.Repositories;

public class MedidorRepository : BaseRepository<Medidor>, IMedidorRepository
{
    public MedidorRepository(AppDbContext context) : base(context) { }

    public async Task<Medidor?> GetByCodigoAsync(string codigo) =>
        await _context.Medidores
            .FirstOrDefaultAsync(m => m.Codigo == codigo);

    public async Task<(IEnumerable<Medidor> Items, int Total)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Medidores.AsQueryable();
        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}