namespace AquaGuard.API.ViewModels.Medidores;

public class CreateMedidorViewModel
{
    public string Codigo { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public double LimiteMensalLitros { get; set; }
}