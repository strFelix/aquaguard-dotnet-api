using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.API.Repositories;

public class AlertaRepository : BaseRepository<Alerta>, IAlertaRepository
{
    public AlertaRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Alerta> Items, int Total)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Alertas
            .Include(a => a.Medidor)
            .OrderByDescending(a => a.DataCriacao)
            .AsQueryable();

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Alerta>> GetPendentesAsync() =>
        await _context.Alertas
            .Include(a => a.Medidor)
            .Where(a => !a.Resolvido)
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();

    public async Task<int> GetTotalAtivosAsync() =>
        await _context.Alertas.CountAsync(a => !a.Resolvido);

    public async Task<int> GetTotalVazamentosAsync() =>
        await _context.Alertas.CountAsync(a => a.Tipo == "Vazamento" && !a.Resolvido);
}