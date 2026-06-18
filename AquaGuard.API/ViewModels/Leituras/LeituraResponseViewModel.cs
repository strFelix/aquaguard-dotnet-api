namespace AquaGuard.API.ViewModels.Leituras;

public class LeituraResponseViewModel
{
    public int Id { get; set; }
    public int MedidorId { get; set; }
    public string CodigoMedidor { get; set; } = string.Empty;
    public double LitrosConsumidos { get; set; }
    public DateTime DataLeitura { get; set; }
    public int UsuarioId { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
}