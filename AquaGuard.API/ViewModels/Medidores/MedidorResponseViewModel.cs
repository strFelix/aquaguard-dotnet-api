namespace AquaGuard.API.ViewModels.Medidores;

public class MedidorResponseViewModel
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public double LimiteMensalLitros { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}