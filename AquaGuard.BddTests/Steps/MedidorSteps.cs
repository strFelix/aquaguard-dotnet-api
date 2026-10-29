using System.Text.Json;
using Reqnroll;
using Xunit;
using AquaGuard.BddTests.Support;

namespace AquaGuard.BddTests.Steps;

[Binding]
public sealed class MedidorSteps
{
    private readonly ApiClient _api;
    private readonly ScenarioContext _context;

    public MedidorSteps(ApiClient api, ScenarioContext context)
    {
        _api = api;
        _context = context;
    }

    [Given(@"que existe um medidor criado para o cenário")]
    [When(@"crio um medidor exclusivo para o cenário")]
    public async Task CreateMeter()
    {
        string? profile = _context.TryGetValue("perfil", out string? currentProfile) ? currentProfile : null;
        await _api.LoginAs("Operador");
        string code = $"BDD-{Guid.NewGuid():N}"[..12];
        string body = JsonSerializer.Serialize(new
        {
            Codigo = code,
            Localizacao = "Teste BDD local",
            LimiteMensalLitros = 10000
        });

        await _api.Send(HttpMethod.Post, "/api/medidores", body);
        Assert.True(
            _api.LastResponse?.StatusCode == System.Net.HttpStatusCode.Created,
            $"Não foi possível criar o medidor de teste. Status: {(int?)_api.LastResponse?.StatusCode}. Corpo: {_api.LastBody}");

        using JsonDocument document = _api.Json();
        _context["medidorId"] = document.RootElement.GetProperty("id").GetInt32();
        _context["medidorCodigo"] = code;
        if (profile is not null)
            await _api.LoginAs(profile);
    }

    [Given(@"que existe um medidor com limite mensal de 500 litros")]
    public async Task CreateMeterWith500LiterLimit()
    {
        string? profile = _context.TryGetValue("perfil", out string? currentProfile) ? currentProfile : null;
        await _api.LoginAs("Operador");
        string code = $"BDD-{Guid.NewGuid():N}"[..12];
        string body = JsonSerializer.Serialize(new
        {
            Codigo = code,
            Localizacao = "Teste BDD limite",
            LimiteMensalLitros = 500
        });

        await _api.Send(HttpMethod.Post, "/api/medidores", body);
        Assert.True(
            _api.LastResponse?.StatusCode == System.Net.HttpStatusCode.Created,
            $"Não foi possível criar o medidor de teste. Status: {(int?)_api.LastResponse?.StatusCode}. Corpo: {_api.LastBody}");

        using JsonDocument document = _api.Json();
        _context["medidorId"] = document.RootElement.GetProperty("id").GetInt32();
        _context["medidorCodigo"] = code;
        if (profile is not null)
            await _api.LoginAs(profile);
    }

    [When(@"busco o medidor criado para o cenário")]
    public async Task GetCreatedMeter()
    {
        int id = (int)_context["medidorId"];
        await _api.Send(HttpMethod.Get, $"/api/medidores/{id}");
    }

    [When(@"atualizo o medidor criado para o cenário")]
    public async Task UpdateCreatedMeter()
    {
        int id = (int)_context["medidorId"];
        string body = JsonSerializer.Serialize(new
        {
            Codigo = $"BDD-{Guid.NewGuid():N}"[..12],
            Localizacao = "Localização atualizada pelo BDD",
            LimiteMensalLitros = 12000
        });
        await _api.Send(HttpMethod.Put, $"/api/medidores/{id}", body);
    }

    [When(@"desativo o medidor criado para o cenário")]
    public async Task DeactivateCreatedMeter()
    {
        int id = (int)_context["medidorId"];
        await _api.LoginAs("Admin");
        await _api.Send(HttpMethod.Delete, $"/api/medidores/{id}");
        _context["medidorDesativado"] = true;
    }

    [Then(@"o medidor do cenário está inativo")]
    public void AssertMeterInactive()
    {
        using JsonDocument document = _api.Json();
        Assert.False(document.RootElement.GetProperty("data").GetProperty("ativo").GetBoolean());
    }
}
