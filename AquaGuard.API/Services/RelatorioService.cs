using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Relatorios;

namespace AquaGuard.API.Services;

public class RelatorioService : IRelatorioService
{
    private readonly ILeituraRepository _leituraRepository;
    private readonly IMedidorRepository _medidorRepository;
    private readonly IAlertaRepository _alertaRepository;

    public RelatorioService(
        ILeituraRepository leituraRepository,
        IMedidorRepository medidorRepository,
        IAlertaRepository alertaRepository)
    {
        _leituraRepository = leituraRepository;
        _medidorRepository = medidorRepository;
        _alertaRepository = alertaRepository;
    }

    public async Task<RelatorioMensalViewModel> GetRelatorioMensalAsync(int ano, int mes)
    {
        DateTime inicio = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime fim = inicio.AddMonths(1).AddTicks(-1);

        IEnumerable<LeituraConsumo> leituras = await _leituraRepository.GetByPeriodoAsync(inicio, fim);
        List<LeituraConsumo> listaLeituras = leituras.ToList();

        double totalConsumo = listaLeituras.Sum(l => l.LitrosConsumidos);
        int diasNoMes = DateTime.DaysInMonth(ano, mes);
        double mediaDiaria = diasNoMes > 0 ? totalConsumo / diasNoMes : 0;

        DateTime inicioMesAnterior = inicio.AddMonths(-1);
        DateTime fimMesAnterior = inicio.AddTicks(-1);
        IEnumerable<LeituraConsumo> leiturasAnterior = await _leituraRepository.GetByPeriodoAsync(inicioMesAnterior, fimMesAnterior);
        double totalAnterior = leiturasAnterior.Sum(l => l.LitrosConsumidos);

        double economiaPercentual = totalAnterior > 0
            ? ((totalAnterior - totalConsumo) / totalAnterior) * 100
            : 0;

        (IEnumerable<Models.Alerta> alertasItems, int _) = await _alertaRepository.GetPagedAsync(1, int.MaxValue);
        int quantidadeAlertas = alertasItems.Count(a => a.DataCriacao >= inicio && a.DataCriacao <= fim);

        return new RelatorioMensalViewModel
        {
            Ano = ano,
            Mes = mes,
            ConsumoTotal = totalConsumo,
            MediaDiaria = mediaDiaria,
            QuantidadeAlertas = quantidadeAlertas,
            EconomiaPercentual = economiaPercentual
        };
    }

    public async Task<RelatorioMedidorViewModel?> GetRelatorioMedidorAsync(int medidorId)
    {
        Medidor? medidor = await _medidorRepository.GetByIdAsync(medidorId);
        if (medidor is null) return null;

        DateTime agora = DateTime.UtcNow;
        double consumoMes = await _leituraRepository.GetConsumoMensalAsync(medidorId, agora.Year, agora.Month);

        DateTime mesAnterior = agora.AddMonths(-1);
        double consumoMesAnterior = await _leituraRepository.GetConsumoMensalAsync(medidorId, mesAnterior.Year, mesAnterior.Month);

        double economiaPercentual = consumoMesAnterior > 0
            ? ((consumoMesAnterior - consumoMes) / consumoMesAnterior) * 100
            : 0;

        return new RelatorioMedidorViewModel
        {
            MedidorId = medidorId,
            CodigoMedidor = medidor.Codigo,
            Localizacao = medidor.Localizacao,
            ConsumoMesAtual = consumoMes,
            ConsumoMesAnterior = consumoMesAnterior,
            LimiteMensal = medidor.LimiteMensalLitros,
            EconomiaPercentual = economiaPercentual,
            DiferencaLitros = consumoMesAnterior - consumoMes,
            Tendencia = economiaPercentual >= 0 ? "Redução" : "Aumento"
        };
    }
}
