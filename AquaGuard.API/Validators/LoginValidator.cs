using AquaGuard.API.ViewModels.Auth;
using FluentValidation;

namespace AquaGuard.API.Validators;

public class LoginValidator : AbstractValidator<LoginRequestViewModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("O email informado não é válido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.");
    }
}