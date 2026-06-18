namespace AquaGuard.API.Models;

public class Medidor
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public double LimiteMensalLitros { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public ICollection<LeituraConsumo> Leituras { get; set; } = new List<LeituraConsumo>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
}