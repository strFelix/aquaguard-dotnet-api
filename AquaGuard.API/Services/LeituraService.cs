using AquaGuard.API.Models;
using AquaGuard.API.Models.Enums;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Leituras;
using AquaGuard.API.ViewModels.Shared;

namespace AquaGuard.API.Services;

public class LeituraService : ILeituraService
{
    private readonly ILeituraRepository _leituraRepository;
    private readonly IMedidorRepository _medidorRepository;
    private readonly IAlertaRepository _alertaRepository;

    public LeituraService(
        ILeituraRepository leituraRepository,
        IMedidorRepository medidorRepository,
        IAlertaRepository alertaRepository)
    {
        _leituraRepository = leituraRepository;
        _medidorRepository = medidorRepository;
        _alertaRepository = alertaRepository;
    }

    public async Task<PaginatedResponseViewModel<LeituraResponseViewModel>> GetAllAsync(int page, int pageSize)
    {
        (IEnumerable<LeituraConsumo> items, int total) = await _leituraRepository.GetPagedAsync(page, pageSize);

        return new PaginatedResponseViewModel<LeituraResponseViewModel>
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = total,
            TotalPages = (int)Math.Ceiling((double)total / pageSize),
            Data = items.Select(MapToViewModel)
        };
    }

    public async Task<IEnumerable<LeituraResponseViewModel>> GetByMedidorAsync(int medidorId)
    {
        IEnumerable<LeituraConsumo> leituras = await _leituraRepository.GetByMedidorAsync(medidorId);
        return leituras.Select(MapToViewModel);
    }

    public async Task<IEnumerable<LeituraResponseViewModel>> GetByPeriodoAsync(DateTime inicio, DateTime fim)
    {
        IEnumerable<LeituraConsumo> leituras = await _leituraRepository.GetByPeriodoAsync(inicio, fim);
        return leituras.Select(MapToViewModel);
    }

    public async Task<LeituraResponseViewModel> CreateAsync(CreateLeituraViewModel model, int usuarioId)
    {
        LeituraConsumo leitura = new LeituraConsumo
        {
            MedidorId = model.MedidorId,
            LitrosConsumidos = model.LitrosConsumidos,
            DataLeitura = model.DataLeitura,
            UsuarioId = usuarioId
        };

        await _leituraRepository.AddAsync(leitura);
        await _leituraRepository.SaveChangesAsync();

        await AnalisarVazamentoAsync(leitura);
        await AnalisarLimiteMensalAsync(leitura);

        LeituraConsumo? leituraCompleta = await _leituraRepository.GetByIdAsync(leitura.Id);

        return MapToViewModel(leituraCompleta!);
    }

    private async Task AnalisarVazamentoAsync(LeituraConsumo leitura)
    {
        IEnumerable<LeituraConsumo> ultimasLeituras = await _leituraRepository.GetUltimosDiasAsync(leitura.MedidorId, 7);
        List<LeituraConsumo> listaLeituras = ultimasLeituras.ToList();

        if (!listaLeituras.Any()) return;

        double media = listaLeituras.Average(l => l.LitrosConsumidos);
        double limiteVazamento = media * 1.5;

        if (leitura.LitrosConsumidos > limiteVazamento)
        {
            double percentualAcima = (leitura.LitrosConsumidos / media - 1) * 100;

            Alerta alerta = new Alerta
            {
                MedidorId = leitura.MedidorId,
                Tipo = TipoAlerta.Vazamento,
                Descricao = $"Consumo de {leitura.LitrosConsumidos:F0}L é {percentualAcima:F0}% acima da média de {media:F0}L dos últimos 7 dias.",
                Status = StatusAlerta.Pendente
            };

            await _alertaRepository.AddAsync(alerta);
            await _alertaRepository.SaveChangesAsync();
        }
    }

    private async Task AnalisarLimiteMensalAsync(LeituraConsumo leitura)
    {
        Medidor? medidor = await _medidorRepository.GetByIdAsync(leitura.MedidorId);
        if (medidor is null) return;

        double consumoMensal = await _leituraRepository.GetConsumoMensalAsync(
            leitura.MedidorId,
            leitura.DataLeitura.Year,
            leitura.DataLeitura.Month);

        if (consumoMensal > medidor.LimiteMensalLitros)
        {
            double percentualExcedido = (consumoMensal / medidor.LimiteMensalLitros - 1) * 100;

            Alerta alerta = new Alerta
            {
                MedidorId = leitura.MedidorId,
                Tipo = TipoAlerta.ConsumoExcessivo,
                Descricao = $"Limite mensal de {medidor.LimiteMensalLitros:F0}L ultrapassado. Consumo atual: {consumoMensal:F0}L ({percentualExcedido:F1}% acima do limite).",
                Status = StatusAlerta.Pendente
            };

            await _alertaRepository.AddAsync(alerta);
            await _alertaRepository.SaveChangesAsync();
        }
    }

    private static LeituraResponseViewModel MapToViewModel(LeituraConsumo l) =>
        new()
        {
            Id = l.Id,
            MedidorId = l.MedidorId,
            CodigoMedidor = l.Medidor?.Codigo ?? string.Empty,
            LitrosConsumidos = l.LitrosConsumidos,
            DataLeitura = l.DataLeitura,
            UsuarioId = l.UsuarioId,
            NomeUsuario = l.Usuario?.Nome ?? string.Empty
        };
}
