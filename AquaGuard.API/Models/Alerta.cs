namespace AquaGuard.API.Models;

public class Alerta
{
    public int Id { get; set; }
    public int MedidorId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Resolvido { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Medidor Medidor { get; set; } = null!;
}