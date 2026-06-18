using AquaGuard.API.Models;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Medidores;
using AquaGuard.API.ViewModels.Shared;

namespace AquaGuard.API.Services;

public class MedidorService : IMedidorService
{
    private readonly IMedidorRepository _medidorRepository;

    public MedidorService(IMedidorRepository medidorRepository)
    {
        _medidorRepository = medidorRepository;
    }

    public async Task<PaginatedResponseViewModel<MedidorResponseViewModel>> GetAllAsync(int page, int pageSize)
    {
        (IEnumerable<Medidor> items, int total) = await _medidorRepository.GetPagedAsync(page, pageSize);

        return new PaginatedResponseViewModel<MedidorResponseViewModel>
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = total,
            TotalPages = (int)Math.Ceiling((double)total / pageSize),
            Data = items.Select(MapToViewModel)
        };
    }

    public async Task<MedidorResponseViewModel?> GetByIdAsync(int id)
    {
        Medidor? medidor = await _medidorRepository.GetByIdAsync(id);
        if (medidor is null) return null;

        return MapToViewModel(medidor);
    }

    public async Task<MedidorResponseViewModel> CreateAsync(CreateMedidorViewModel model)
    {
        Medidor medidor = new Medidor
        {
            Codigo = model.Codigo,
            Localizacao = model.Localizacao,
            LimiteMensalLitros = model.LimiteMensalLitros
        };

        await _medidorRepository.AddAsync(medidor);
        await _medidorRepository.SaveChangesAsync();

        return MapToViewModel(medidor);
    }

    public async Task<MedidorResponseViewModel?> UpdateAsync(int id, UpdateMedidorViewModel model)
    {
        Medidor? medidor = await _medidorRepository.GetByIdAsync(id);
        if (medidor is null) return null;

        medidor.Codigo = model.Codigo;
        medidor.Localizacao = model.Localizacao;
        medidor.LimiteMensalLitros = model.LimiteMensalLitros;

        await _medidorRepository.UpdateAsync(medidor);
        await _medidorRepository.SaveChangesAsync();

        return MapToViewModel(medidor);
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        Medidor? medidor = await _medidorRepository.GetByIdAsync(id);
        if (medidor is null) return false;

        medidor.Ativo = false;

        await _medidorRepository.UpdateAsync(medidor);
        await _medidorRepository.SaveChangesAsync();

        return true;
    }

    private static MedidorResponseViewModel MapToViewModel(Medidor m) =>
        new()
        {
            Id = m.Id,
            Codigo = m.Codigo,
            Localizacao = m.Localizacao,
            LimiteMensalLitros = m.LimiteMensalLitros,
            Ativo = m.Ativo,
            DataCriacao = m.DataCriacao
        };
}
