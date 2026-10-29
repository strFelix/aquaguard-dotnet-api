using System.Text.Json;
using Reqnroll;
using Xunit;
using AquaGuard.BddTests.Support;

namespace AquaGuard.BddTests.Steps;

[Binding]
public sealed class AlertaSteps
{
    private readonly ApiClient _api;
    private readonly ScenarioContext _context;

    public AlertaSteps(ApiClient api, ScenarioContext context)
    {
        _api = api;
        _context = context;
    }

    [Given(@"que foi gerado um alerta de vazamento para o cenário")]
    public async Task CreateLeakAlert()
    {
        await _api.LoginAs("Operador");
        await CreateMeterForAlert();
        await RegisterReading(100);
        await RegisterReading(250);

        await _api.Send(HttpMethod.Get, "/api/alertas/pendentes");
        using JsonDocument document = _api.Json();
        JsonElement alert = FindPendingAlert(document.RootElement, "Vazamento");
        _context["alertaId"] = alert.GetProperty("id").GetInt32();
    }

    [When(@"resolvo o alerta criado para o cenário")]
    public async Task ResolveScenarioAlert()
    {
        int alertId = (int)_context["alertaId"];
        await _api.Send(HttpMethod.Put, $"/api/alertas/{alertId}/resolver");
    }

    [When(@"tento resolver o alerta criado como ""(.*)""")]
    public async Task TryResolveAs(string profile)
    {
        await _api.LoginAs(profile);
        await ResolveScenarioAlert();
    }

    [When(@"tento resolver um alerta inexistente como ""(.*)""")]
    public async Task ResolveMissingAlert(string profile)
    {
        await _api.LoginAs(profile);
        await _api.Send(HttpMethod.Put, "/api/alertas/2147483647/resolver");
    }

    [Then(@"deve existir um alerta pendente do tipo ""(.*)"" para o medidor do cenário")]
    public async Task AssertPendingAlert(string type)
    {
        int meterId = (int)_context["medidorId"];
        await _api.Send(HttpMethod.Get, "/api/alertas/pendentes");
        using JsonDocument document = _api.Json();
        JsonElement alert = FindPendingAlert(document.RootElement, type);
        Assert.Equal(meterId, alert.GetProperty("medidorId").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(alert.GetProperty("descricao").GetString()));
    }

    [Then(@"não deve existir alerta pendente para o medidor do cenário")]
    public async Task AssertNoPendingAlertForMeter()
    {
        int meterId = (int)_context["medidorId"];
        await _api.Send(HttpMethod.Get, "/api/alertas/pendentes");
        using JsonDocument document = _api.Json();
        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
        Assert.DoesNotContain(
            document.RootElement.EnumerateArray(),
            alert => alert.GetProperty("medidorId").GetInt32() == meterId);
    }

    [Then(@"a descrição do alerta deve conter o percentual de desvio")]
    public async Task AssertLeakDescription()
    {
        await _api.Send(HttpMethod.Get, "/api/alertas/pendentes");
        using JsonDocument document = _api.Json();
        JsonElement alert = FindPendingAlert(document.RootElement, "Vazamento");
        Assert.Contains("%", alert.GetProperty("descricao").GetString() ?? string.Empty);
    }

    [Then(@"o alerta do cenário está resolvido")]
    public void AssertAlertResolved()
    {
        using JsonDocument document = _api.Json();
        Assert.True(document.RootElement.GetProperty("resolvido").GetBoolean());
    }

    private async Task CreateMeterForAlert()
    {
        string code = $"BDD-{Guid.NewGuid():N}"[..12];
        string body = JsonSerializer.Serialize(new
        {
            Codigo = code,
            Localizacao = "Teste BDD alerta",
            LimiteMensalLitros = 500
        });
        await _api.Send(HttpMethod.Post, "/api/medidores", body);
        Assert.True(
            _api.LastResponse?.StatusCode == System.Net.HttpStatusCode.Created,
            $"Falha ao criar medidor para alerta. Corpo: {_api.LastBody}");

        using JsonDocument document = _api.Json();
        _context["medidorId"] = document.RootElement.GetProperty("id").GetInt32();
        _context["medidorCodigo"] = code;
    }

    private async Task RegisterReading(double liters)
    {
        string body = JsonSerializer.Serialize(new
        {
            MedidorId = (int)_context["medidorId"],
            LitrosConsumidos = liters,
            DataLeitura = DateTime.UtcNow.ToString("O")
        });
        await _api.Send(HttpMethod.Post, "/api/leituras", body);
        Assert.True(
            _api.LastResponse?.StatusCode == System.Net.HttpStatusCode.OK,
            $"Falha ao registrar leitura para alerta. Corpo: {_api.LastBody}");
    }

    private JsonElement FindPendingAlert(JsonElement alerts, string type)
    {
        Assert.Equal(JsonValueKind.Array, alerts.ValueKind);
        foreach (JsonElement alert in alerts.EnumerateArray())
        {
            if (alert.GetProperty("tipo").GetString() == type
                && !alert.GetProperty("resolvido").GetBoolean())
            {
                if (_context.TryGetValue("medidorId", out int meterId)
                    && alert.GetProperty("medidorId").GetInt32() != meterId)
                    continue;
                return alert;
            }
        }

        string meterDescription = _context.TryGetValue("medidorId", out int expectedMeterId)
            ? $" para o medidor {expectedMeterId}"
            : string.Empty;
        throw new Xunit.Sdk.XunitException($"Não foi encontrado alerta pendente '{type}'{meterDescription}. Corpo: {alerts}");
    }
}
