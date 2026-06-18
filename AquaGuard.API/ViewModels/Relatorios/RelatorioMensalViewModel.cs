namespace AquaGuard.API.ViewModels.Relatorios;

public class RelatorioMensalViewModel
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public double ConsumoTotal { get; set; }
    public double MediaDiaria { get; set; }
    public int QuantidadeAlertas { get; set; }
    public double EconomiaPercentual { get; set; }
}