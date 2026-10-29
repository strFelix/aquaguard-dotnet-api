using Reqnroll;

namespace AquaGuard.BddTests.Support;

[Binding]
public sealed class CleanupHooks
{
    [AfterScenario("limpar", Order = 100)]
    public async Task DeactivateCreatedMeter(ScenarioContext context, ApiClient api)
    {
        if (!context.TryGetValue("medidorId", out int medidorId))
            return;
        if (context.TryGetValue("medidorDesativado", out bool deactivated) && deactivated)
            return;

        await api.LoginAs("Admin");
        await api.Send(HttpMethod.Delete, $"/api/medidores/{medidorId}");

        if (api.LastResponse is null || !api.LastResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Não foi possível desativar o medidor de teste {medidorId}. Corpo: {api.LastBody}");
        }
    }
}
