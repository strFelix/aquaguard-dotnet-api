using AquaGuard.API.Models;
using AquaGuard.API.Models.Enums;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Alertas;
using AquaGuard.API.ViewModels.Shared;

namespace AquaGuard.API.Services;

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepository _alertaRepository;

    public AlertaService(IAlertaRepository alertaRepository)
    {
        _alertaRepository = alertaRepository;
    }

    public async Task<PaginatedResponseViewModel<AlertaResponseViewModel>> GetAllAsync(int page, int pageSize)
    {
        (IEnumerable<Alerta> items, int total) = await _alertaRepository.GetPagedAsync(page, pageSize);

        return new PaginatedResponseViewModel<AlertaResponseViewModel>
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = total,
            TotalPages = (int)Math.Ceiling((double)total / pageSize),
            Data = items.Select(MapToViewModel)
        };
    }

    public async Task<IEnumerable<AlertaResponseViewModel>> GetPendentesAsync()
    {
        IEnumerable<Alerta> alertas = await _alertaRepository.GetPendentesAsync();
        return alertas.Select(MapToViewModel);
    }

    public async Task<AlertaResponseViewModel?> ResolverAsync(int id)
    {
        Alerta? alerta = await _alertaRepository.GetByIdAsync(id);
        if (alerta is null) return null;

        alerta.Status = StatusAlerta.Resolvido;

        await _alertaRepository.UpdateAsync(alerta);
        await _alertaRepository.SaveChangesAsync();

        return MapToViewModel(alerta);
    }

    private static AlertaResponseViewModel MapToViewModel(Alerta a) =>
        new()
        {
            Id = a.Id,
            MedidorId = a.MedidorId,
            CodigoMedidor = a.Medidor?.Codigo ?? string.Empty,
            Tipo = a.Tipo.ToString(),
            Descricao = a.Descricao,
            Resolvido = a.Status == StatusAlerta.Resolvido,
            DataCriacao = a.DataCriacao
        };
}
