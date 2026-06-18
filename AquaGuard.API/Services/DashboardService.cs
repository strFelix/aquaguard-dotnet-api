using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Relatorios;

namespace AquaGuard.API.Services;

public class DashboardService : IDashboardService
{
    private readonly ILeituraRepository _leituraRepository;
    private readonly IMedidorRepository _medidorRepository;
    private readonly IAlertaRepository _alertaRepository;

    public DashboardService(
        ILeituraRepository leituraRepository,
        IMedidorRepository medidorRepository,
        IAlertaRepository alertaRepository)
    {
        _leituraRepository = leituraRepository;
        _medidorRepository = medidorRepository;
        _alertaRepository = alertaRepository;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        DateTime agora = DateTime.UtcNow;
        DateTime inicio = new DateTime(agora.Year, agora.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        IEnumerable<Medidor> medidores = await _medidorRepository.GetAllAsync();
        int medidoresAtivos = medidores.Count(m => m.Ativo);

        IEnumerable<LeituraConsumo> leiturasMes = await _leituraRepository.GetByPeriodoAsync(inicio, agora);
        double consumoMes = leiturasMes.Sum(l => l.LitrosConsumidos);

        DateTime inicioAnterior = inicio.AddMonths(-1);
        DateTime fimAnterior = inicio.AddTicks(-1);
        IEnumerable<LeituraConsumo> leiturasAnterior = await _leituraRepository.GetByPeriodoAsync(inicioAnterior, fimAnterior);
        double consumoAnterior = leiturasAnterior.Sum(l => l.LitrosConsumidos);

        double economiaPercentual = consumoAnterior > 0
            ? ((consumoAnterior - consumoMes) / consumoAnterior) * 100
            : 0;

        int alertasAtivos = await _alertaRepository.GetTotalAtivosAsync();
        int vazamentos = await _alertaRepository.GetTotalVazamentosAsync();

        return new DashboardViewModel
        {
            ConsumoMes = consumoMes,
            EconomiaPercentual = economiaPercentual,
            AlertasAtivos = alertasAtivos,
            VazamentosDetectados = vazamentos,
            MedidoresMonitorados = medidoresAtivos
        };
    }
}
