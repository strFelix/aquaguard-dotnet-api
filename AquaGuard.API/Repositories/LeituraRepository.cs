using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.API.Repositories;

public class LeituraRepository : BaseRepository<LeituraConsumo>, ILeituraRepository
{
    public LeituraRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<LeituraConsumo> Items, int Total)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Leituras
            .Include(l => l.Medidor)
            .Include(l => l.Usuario)
            .OrderByDescending(l => l.DataLeitura)
            .AsQueryable();

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<LeituraConsumo>> GetByMedidorAsync(int medidorId) =>
        await _context.Leituras
            .Include(l => l.Medidor)
            .Where(l => l.MedidorId == medidorId)
            .OrderByDescending(l => l.DataLeitura)
            .ToListAsync();

    public async Task<IEnumerable<LeituraConsumo>> GetByPeriodoAsync(DateTime inicio, DateTime fim) =>
        await _context.Leituras
            .Include(l => l.Medidor)
            .Where(l => l.DataLeitura >= inicio && l.DataLeitura <= fim)
            .OrderByDescending(l => l.DataLeitura)
            .ToListAsync();

    public async Task<IEnumerable<LeituraConsumo>> GetUltimosDiasAsync(int medidorId, int dias)
    {
        var dataLimite = DateTime.UtcNow.AddDays(-dias);
        return await _context.Leituras
            .Where(l => l.MedidorId == medidorId && l.DataLeitura >= dataLimite)
            .OrderByDescending(l => l.DataLeitura)
            .ToListAsync();
    }

    public async Task<double> GetConsumoMensalAsync(int medidorId, int ano, int mes) =>
        await _context.Leituras
            .Where(l => l.MedidorId == medidorId
                     && l.DataLeitura.Year == ano
                     && l.DataLeitura.Month == mes)
            .SumAsync(l => l.LitrosConsumidos);
}