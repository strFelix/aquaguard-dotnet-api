using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.ViewModels.Leituras;
using FluentValidation;

namespace AquaGuard.API.Validators;

public class CreateLeituraValidator : AbstractValidator<CreateLeituraViewModel>
{
    private readonly IMedidorRepository _medidorRepository;

    public CreateLeituraValidator(IMedidorRepository medidorRepository)
    {
        _medidorRepository = medidorRepository;

        RuleFor(x => x.LitrosConsumidos)
            .GreaterThan(0).WithMessage("A quantidade de litros consumidos deve ser maior que zero.");

        RuleFor(x => x.DataLeitura)
            .NotEmpty().WithMessage("A data da leitura é obrigatória.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data da leitura não pode ser futura.");

        RuleFor(x => x.MedidorId)
            .GreaterThan(0).WithMessage("O medidor informado é inválido.")
            .MustAsync(MedidorExistir).WithMessage("O medidor informado não existe.");
    }

    private async Task<bool> MedidorExistir(int medidorId, CancellationToken cancellationToken)
    {
        Models.Medidor? medidor = await _medidorRepository.GetByIdAsync(medidorId);
        return medidor is not null;
    }
}