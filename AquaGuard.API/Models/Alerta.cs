using AquaGuard.API.Models.Enums;

namespace AquaGuard.API.Models;

public class Alerta
{
    public int Id { get; set; }
    public int MedidorId { get; set; }
    public TipoAlerta Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public StatusAlerta Status { get; set; } = StatusAlerta.Pendente;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public Medidor Medidor { get; set; } = null!;
}