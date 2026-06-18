using AquaGuard.API.ViewModels.Relatorios;

namespace AquaGuard.API.Services.Interfaces;

public interface IRelatorioService
{
    Task<RelatorioMensalViewModel> GetRelatorioMensalAsync(int ano, int mes);
    Task<RelatorioMedidorViewModel?> GetRelatorioMedidorAsync(int medidorId);
}
