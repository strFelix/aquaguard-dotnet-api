namespace AquaGuard.API.ViewModels.Relatorios;

public class RelatorioMedidorViewModel
{
    public int MedidorId { get; set; }
    public string CodigoMedidor { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty;
    public double ConsumoMesAtual { get; set; }
    public double ConsumoMesAnterior { get; set; }
    public double LimiteMensal { get; set; }
    public double EconomiaPercentual { get; set; }
    public double DiferencaLitros { get; set; }
    public string Tendencia { get; set; } = string.Empty;
}