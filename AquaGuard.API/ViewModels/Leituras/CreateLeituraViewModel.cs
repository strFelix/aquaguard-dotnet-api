namespace AquaGuard.API.ViewModels.Leituras;

public class CreateLeituraViewModel
{
    public int MedidorId { get; set; }
    public double LitrosConsumidos { get; set; }
    public DateTime DataLeitura { get; set; }
}