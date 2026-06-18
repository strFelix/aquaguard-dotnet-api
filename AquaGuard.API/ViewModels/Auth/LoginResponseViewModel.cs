namespace AquaGuard.API.ViewModels.Auth;

public class LoginResponseViewModel
{
    public string Token { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime Expiracao { get; set; }
}