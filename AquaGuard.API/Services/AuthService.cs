using AquaGuard.API.Authentication;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services.Interfaces;
using AquaGuard.API.ViewModels.Auth;

namespace AquaGuard.API.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        JwtTokenGenerator jwtTokenGenerator,
        JwtSettings jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtSettings = jwtSettings;
    }

    public async Task<LoginResponseViewModel?> LoginAsync(LoginRequestViewModel model)
    {
        Models.Usuario? usuario = await _usuarioRepository.GetByEmailAsync(model.Email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(model.Senha, usuario.SenhaHash))
            return null;

        string token = _jwtTokenGenerator.GerarToken(usuario);

        return new LoginResponseViewModel
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Role = usuario.Role.ToString(),
            Expiracao = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours)
        };
    }
}
