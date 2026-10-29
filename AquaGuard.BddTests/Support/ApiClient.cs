using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AquaGuard.BddTests.Support;

public sealed class ApiClient
{
    private static readonly ConcurrentDictionary<string, string> TokenCache = new();
    private static readonly SemaphoreSlim LoginLock = new(1, 1);
    private readonly HttpClient _http;

    public HttpResponseMessage? LastResponse { get; private set; }
    public string LastBody { get; private set; } = string.Empty;

    public ApiClient()
    {
        string baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5187";
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri? baseUri))
            throw new InvalidOperationException("BASE_URL deve ser uma URL absoluta.");

        var handler = new HttpClientHandler();
        if (bool.TryParse(Environment.GetEnvironmentVariable("INSECURE_SSL"), out bool insecureSsl) && insecureSsl)
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUri.AbsoluteUri.TrimEnd('/') + "/")
        };
    }

    public async Task LoginAs(string perfil)
    {
        string key = perfil.ToUpperInvariant();
        string email = Environment.GetEnvironmentVariable($"{key}_EMAIL")
            ?? throw new InvalidOperationException($"A variável {key}_EMAIL não está definida.");
        string password = Environment.GetEnvironmentVariable($"{key}_PASSWORD")
            ?? throw new InvalidOperationException($"A variável {key}_PASSWORD não está definida.");
        string cacheKey = $"{_http.BaseAddress}|{key}";

        if (!TokenCache.TryGetValue(cacheKey, out string? token))
        {
            await LoginLock.WaitAsync();
            try
            {
                if (!TokenCache.TryGetValue(cacheKey, out token))
                {
                    using HttpResponseMessage response = await _http.PostAsJsonAsync(
                        "/api/auth/login",
                        new { Email = email, Senha = password });
                    string body = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            $"Login como {perfil} falhou com {(int)response.StatusCode}: {body}");
                    }

                    using JsonDocument document = JsonDocument.Parse(body);
                    token = FindToken(document.RootElement)
                        ?? throw new InvalidOperationException("A resposta de login não contém token.");
                    TokenCache[cacheKey] = token;
                }
            }
            finally
            {
                LoginLock.Release();
            }
        }

        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException($"Login como {perfil} não retornou um token utilizável.");

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void Logout() => _http.DefaultRequestHeaders.Authorization = null;

    public async Task Send(HttpMethod method, string url, string? jsonBody = null)
    {
        using var request = new HttpRequestMessage(method, url);
        if (jsonBody is not null)
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _http.SendAsync(request);
        LastResponse = response;
        LastBody = await response.Content.ReadAsStringAsync();
    }

    public JsonDocument Json() => JsonDocument.Parse(LastBody);

    private static string? FindToken(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (property.Name.Equals("token", StringComparison.OrdinalIgnoreCase)
                    && property.Value.ValueKind == JsonValueKind.String)
                    return property.Value.GetString();

                string? nested = FindToken(property.Value);
                if (nested is not null)
                    return nested;
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in element.EnumerateArray())
            {
                string? nested = FindToken(item);
                if (nested is not null)
                    return nested;
            }
        }

        return null;
    }
}
