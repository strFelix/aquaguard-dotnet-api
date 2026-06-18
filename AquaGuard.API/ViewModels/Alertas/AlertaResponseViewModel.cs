namespace AquaGuard.API.ViewModels.Alertas;

public class AlertaResponseViewModel
{
    public int Id { get; set; }
    public int MedidorId { get; set; }
    public string CodigoMedidor { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Resolvido { get; set; }
    public DateTime DataCriacao { get; set; }
}