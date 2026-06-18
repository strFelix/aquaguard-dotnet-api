namespace AquaGuard.API.ViewModels.Relatorios;

public class DashboardViewModel
{
    public double ConsumoMes { get; set; }
    public double EconomiaPercentual { get; set; }
    public int AlertasAtivos { get; set; }
    public int VazamentosDetectados { get; set; }
    public int MedidoresMonitorados { get; set; }
}