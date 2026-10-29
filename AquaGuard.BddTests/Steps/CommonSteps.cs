using Json.Schema;
using Reqnroll;
using System.Text.Json;
using System.Collections.Concurrent;
using Xunit;
using AquaGuard.BddTests.Support;

namespace AquaGuard.BddTests.Steps;

[Binding]
public sealed class CommonSteps
{
    private static readonly ConcurrentDictionary<string, Lazy<JsonSchema>> SchemaCache = new();
    private readonly ApiClient _api;
    private readonly ScenarioContext _context;

    public CommonSteps(ApiClient api, ScenarioContext context)
    {
        _api = api;
        _context = context;
    }

    [Given(@"que estou autenticado como ""(.*)""")]
    public async Task Authenticate(string profile)
    {
        _context["perfil"] = profile;
        await _api.LoginAs(profile);
    }

    [Given(@"que não estou autenticado")]
    public void Unauthenticated() => _api.Logout();

    [When(@"faço login como ""(.*)""")]
    public async Task Login(string profile)
    {
        string key = profile.ToUpperInvariant();
        string email = Environment.GetEnvironmentVariable($"{key}_EMAIL")
            ?? throw new InvalidOperationException($"A variável {key}_EMAIL não está definida.");
        string password = Environment.GetEnvironmentVariable($"{key}_PASSWORD")
            ?? throw new InvalidOperationException($"A variável {key}_PASSWORD não está definida.");
        string body = JsonSerializer.Serialize(new { Email = email, Senha = password });
        await _api.Send(HttpMethod.Post, "/api/auth/login", body);
    }

    [When(@"envio GET para ""(.*)""")]
    public async Task Get(string url) => await _api.Send(HttpMethod.Get, Resolve(url));

    [When(@"envio POST para ""(.*)"" com o corpo:")]
    public async Task Post(string url, string body) =>
        await _api.Send(HttpMethod.Post, Resolve(url), Resolve(body));

    [When(@"envio PUT para ""(.*)"" com o corpo:")]
    public async Task Put(string url, string body) =>
        await _api.Send(HttpMethod.Put, Resolve(url), Resolve(body));

    [When(@"envio DELETE para ""(.*)""")]
    public async Task Delete(string url) => await _api.Send(HttpMethod.Delete, Resolve(url));

    [Then(@"a resposta deve ter status (\d+)")]
    public void AssertStatus(int expected)
    {
        Assert.NotNull(_api.LastResponse);
        Assert.True(
            expected == (int)_api.LastResponse!.StatusCode,
            $"Esperado {expected}, recebido {(int)_api.LastResponse.StatusCode}. Corpo: {_api.LastBody}");
    }

    [Then(@"o corpo deve ser válido contra o schema ""(.*)""")]
    public void AssertSchema(string name)
    {
        string schemaPath = Path.Combine(AppContext.BaseDirectory, "Schemas", $"{name}.schema.json");
        JsonSchema schema = SchemaCache.GetOrAdd(
            schemaPath,
            path => new Lazy<JsonSchema>(() => JsonSchema.FromFile(path))).Value;
        using var document = _api.Json();
        EvaluationResults result = schema.Evaluate(
            document.RootElement,
            new EvaluationOptions { OutputFormat = OutputFormat.List });

        Assert.True(result.IsValid, $"Contrato violado ({name}). Corpo: {_api.LastBody}");
    }

    [Then(@"o campo ""(.*)"" deve ser ""(.*)""")]
    public void AssertField(string path, string expected)
    {
        using var document = _api.Json();
        JsonElement element = FindElement(document.RootElement, path);
        Assert.True(
            string.Equals(expected, element.ToString(), StringComparison.OrdinalIgnoreCase),
            $"Esperado '{expected}', recebido '{element}'.");
    }

    [Then(@"a resposta deve conter o campo ""(.*)""")]
    public void AssertFieldExists(string path)
    {
        using var document = _api.Json();
        _ = FindElement(document.RootElement, path);
    }

    [Then(@"a resposta deve incluir o cabeçalho Location")]
    public void AssertLocationHeader()
    {
        Assert.NotNull(_api.LastResponse);
        Assert.NotNull(_api.LastResponse!.Headers.Location);
    }

    private string Resolve(string value)
    {
        string result = value
            .Replace("__NOW_UTC__", DateTime.UtcNow.ToString("O"), StringComparison.Ordinal)
            .Replace("__CURRENT_YEAR__", DateTime.UtcNow.Year.ToString(), StringComparison.Ordinal)
            .Replace("__CURRENT_MONTH__", DateTime.UtcNow.Month.ToString(), StringComparison.Ordinal)
            .Replace("__UNIQUE_CODE__", $"BDD-{Guid.NewGuid():N}"[..12], StringComparison.Ordinal);
        if (_context.TryGetValue("medidorId", out int medidorId))
            result = result.Replace("__MEDIDOR_ID__", medidorId.ToString(), StringComparison.Ordinal);
        if (_context.TryGetValue("medidorCodigo", out string? codigo))
            result = result.Replace("__MEDIDOR_CODIGO__", codigo, StringComparison.Ordinal);
        return result;
    }

    private static JsonElement FindElement(JsonElement root, string path)
    {
        JsonElement current = root;
        foreach (string part in path.Split('.'))
        {
            if (int.TryParse(part, out int index))
                current = current[index];
            else
                current = current.GetProperty(part);
        }
        return current;
    }
}
