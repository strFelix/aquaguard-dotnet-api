using Reqnroll;
using Reqnroll.BoDi;

namespace AquaGuard.BddTests.Support;

[Binding]
public sealed class Hooks
{
    [BeforeTestRun]
    public static void LoadEnvironment()
    {
        LoadEnvironmentFile();

        string baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5187";
        if (baseUrl.Contains("aquaguardapi-evh6dzbvg4b8d7cd", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("BASE_URL aponta para PRODUÇÃO. Execução abortada.");

        Environment.SetEnvironmentVariable("BASE_URL", baseUrl);
    }

    [BeforeScenario]
    public void RegisterApiClient(IObjectContainer container) =>
        container.RegisterInstanceAs(new ApiClient());

    private static void LoadEnvironmentFile()
    {
        string[] starts = [AppContext.BaseDirectory, Directory.GetCurrentDirectory()];
        foreach (string start in starts)
        {
            string? directory = start;
            while (directory is not null)
            {
                string path = Path.Combine(directory, ".env");
                if (File.Exists(path))
                {
                    foreach (string line in File.ReadLines(path))
                    {
                        string trimmed = line.Trim();
                        if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                            continue;

                        int separator = trimmed.IndexOf('=');
                        if (separator <= 0)
                            throw new FormatException($"Linha inválida no arquivo .env: {trimmed}");

                        string name = trimmed[..separator].Trim();
                        string value = trimmed[(separator + 1)..].Trim();
                        if (name.Length == 0)
                            throw new FormatException("Foi encontrada uma variável sem nome no arquivo .env.");
                        if (value.Length >= 2 && value[0] == value[^1] && (value[0] == '"' || value[0] == '\''))
                            value = value[1..^1];

                        if (Environment.GetEnvironmentVariable(name) is null)
                            Environment.SetEnvironmentVariable(name, value);
                    }

                    return;
                }

                directory = Directory.GetParent(directory)?.FullName;
            }
        }
    }
}
