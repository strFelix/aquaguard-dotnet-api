using AquaGuard.API.ViewModels.Alertas;
using FluentValidation;

namespace AquaGuard.API.Validators;

public class ResolverAlertaValidator : AbstractValidator<ResolverAlertaViewModel>
{
    public ResolverAlertaValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("O identificador do alerta é inválido.");
    }
}