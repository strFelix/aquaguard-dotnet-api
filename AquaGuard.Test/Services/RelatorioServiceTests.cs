using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Repositories;
using AquaGuard.API.Services;
using AquaGuard.API.ViewModels.Relatorios;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AquaGuard.Tests.Services;

public class RelatorioServiceTests
{
    private static AppDbContext CriarContextoEmMemoria()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetRelatorioMensalAsync_DeveCalcularConsumoTotalCorretamente()
    {
        AppDbContext context = CriarContextoEmMemoria();

        Medidor medidor = new Medidor
        {
            Codigo = "MED-RELATORIO-01",
            Localizacao = "Setor Testes",
            LimiteMensalLitros = 100000,
            Ativo = true
        };

        context.Medidores.Add(medidor);
        await context.SaveChangesAsync();

        DateTime dataReferencia = new DateTime(2024, 5, 10, 8, 0, 0, DateTimeKind.Utc);

        context.Leituras.AddRange(
            new LeituraConsumo { MedidorId = medidor.Id, LitrosConsumidos = 1000, DataLeitura = dataReferencia, UsuarioId = 1 },
            new LeituraConsumo { MedidorId = medidor.Id, LitrosConsumidos = 1500, DataLeitura = dataReferencia.AddDays(1), UsuarioId = 1 }
        );

        await context.SaveChangesAsync();

        LeituraRepository leituraRepository = new LeituraRepository(context);
        MedidorRepository medidorRepository = new MedidorRepository(context);
        AlertaRepository alertaRepository = new AlertaRepository(context);

        RelatorioService service = new RelatorioService(leituraRepository, medidorRepository, alertaRepository);

        RelatorioMensalViewModel resultado = await service.GetRelatorioMensalAsync(2024, 5);

        Assert.Equal(2500, resultado.ConsumoTotal);
        Assert.Equal(2024, resultado.Ano);
        Assert.Equal(5, resultado.Mes);
    }
}