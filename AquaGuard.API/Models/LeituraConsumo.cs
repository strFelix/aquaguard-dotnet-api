namespace AquaGuard.API.Models;

public class LeituraConsumo
{
    public int Id { get; set; }
    public int MedidorId { get; set; }
    public double LitrosConsumidos { get; set; }
    public DateTime DataLeitura { get; set; }
    public int UsuarioId { get; set; }

    public Medidor Medidor { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}