using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.ViewModels.Medidores;
using FluentValidation;

namespace AquaGuard.API.Validators;

public class CreateMedidorValidator : AbstractValidator<CreateMedidorViewModel>
{
    private readonly IMedidorRepository _medidorRepository;

    public CreateMedidorValidator(IMedidorRepository medidorRepository)
    {
        _medidorRepository = medidorRepository;

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código do medidor é obrigatório.")
            .MaximumLength(50).WithMessage("O código deve ter no máximo 50 caracteres.")
            .MustAsync(SerCodigoUnico).WithMessage("Já existe um medidor cadastrado com este código.");

        RuleFor(x => x.Localizacao)
            .NotEmpty().WithMessage("A localização é obrigatória.")
            .MaximumLength(200).WithMessage("A localização deve ter no máximo 200 caracteres.");

        RuleFor(x => x.LimiteMensalLitros)
            .GreaterThan(0).WithMessage("O limite mensal deve ser maior que zero.");
    }

    private async Task<bool> SerCodigoUnico(string codigo, CancellationToken cancellationToken)
    {
        Models.Medidor? medidorExistente = await _medidorRepository.GetByCodigoAsync(codigo);
        return medidorExistente is null;
    }
}