using System.Globalization;
using System.Text.Json;
using Reqnroll;
using Xunit;
using AquaGuard.BddTests.Support;

namespace AquaGuard.BddTests.Steps;

[Binding]
public sealed class LeituraSteps
{
    private readonly ApiClient _api;
    private readonly ScenarioContext _context;

    public LeituraSteps(ApiClient api, ScenarioContext context)
    {
        _api = api;
        _context = context;
    }

    [When(@"registro uma leitura de ([\d.,]+) litros nesse medidor")]
    public async Task RegisterReading(string litersText)
    {
        int meterId = (int)_context["medidorId"];
        double liters = double.Parse(litersText.Replace(',', '.'), CultureInfo.InvariantCulture);
        string body = JsonSerializer.Serialize(new
        {
            MedidorId = meterId,
            LitrosConsumidos = liters,
            DataLeitura = DateTime.UtcNow.ToString("O")
        });
        await _api.Send(HttpMethod.Post, "/api/leituras", body);
    }

    [Then(@"a leitura deve identificar o usuário autenticado")]
    public void AssertReadingUser()
    {
        using JsonDocument document = _api.Json();
        JsonElement root = document.RootElement;
        Assert.True(root.GetProperty("usuarioId").GetInt32() > 0);
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("nomeUsuario").GetString()));
    }
}
