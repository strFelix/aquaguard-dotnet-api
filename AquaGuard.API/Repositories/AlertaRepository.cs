using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Models.Enums;
using AquaGuard.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.API.Repositories;

public class AlertaRepository : BaseRepository<Alerta>, IAlertaRepository
{
    public AlertaRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Alerta> Items, int Total)> GetPagedAsync(int page, int pageSize)
    {
        IQueryable<Alerta> query = _context.Alertas
            .Include(a => a.Medidor)
            .OrderByDescending(a => a.DataCriacao);

        int total = await query.CountAsync();
        List<Alerta> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Alerta>> GetPendentesAsync() =>
        await _context.Alertas
            .Include(a => a.Medidor)
            .Where(a => a.Status == StatusAlerta.Pendente)
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();

    public async Task<int> GetTotalAtivosAsync() =>
        await _context.Alertas
            .CountAsync(a => a.Status == StatusAlerta.Pendente);

    public async Task<int> GetTotalVazamentosAsync() =>
        await _context.Alertas
            .CountAsync(a => a.Tipo == TipoAlerta.Vazamento && a.Status == StatusAlerta.Pendente);
}