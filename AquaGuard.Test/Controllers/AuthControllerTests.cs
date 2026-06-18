using AquaGuard.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AquaGuard.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_ReturnsHttpStatusCode200()
    {
        var payload = new
        {
            Email = "admin@aquaguard.com",
            Senha = "Admin@123"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}