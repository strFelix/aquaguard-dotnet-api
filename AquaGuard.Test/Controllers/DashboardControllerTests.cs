using AquaGuard.Tests.Fixtures;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AquaGuard.Tests.Controllers;

public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AutenticarAsync()
    {
        var payload = new { Email = "admin@aquaguard.com", Senha = "Admin@123" };
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", payload);
        var resultado = await response.Content.ReadFromJsonAsync<LoginResponseTeste>();

        if (resultado?.Data?.Token is not null)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", resultado.Data.Token);
        }
    }

    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        await AutenticarAsync();

        HttpResponseMessage response = await _client.GetAsync("/api/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private class LoginResponseTeste
    {
        public LoginDataTeste? Data { get; set; }
    }

    private class LoginDataTeste
    {
        public string? Token { get; set; }
    }
}