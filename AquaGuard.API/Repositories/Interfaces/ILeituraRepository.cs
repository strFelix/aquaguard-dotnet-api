using AquaGuard.API.Models;

namespace AquaGuard.API.Repositories.Interfaces;

public interface ILeituraRepository : IRepository<LeituraConsumo>
{
    Task<(IEnumerable<LeituraConsumo> Items, int Total)> GetPagedAsync(int page, int pageSize);
    Task<IEnumerable<LeituraConsumo>> GetByMedidorAsync(int medidorId);
    Task<IEnumerable<LeituraConsumo>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
    Task<IEnumerable<LeituraConsumo>> GetUltimosDiasAsync(int medidorId, int dias);
    Task<double> GetConsumoMensalAsync(int medidorId, int ano, int mes);
}