using AquaGuard.API.ViewModels.Auth;

namespace AquaGuard.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseViewModel?> LoginAsync(LoginRequestViewModel model);
}
