using System.Reflection;
using AquaGuard.API.Data;
using AquaGuard.API.Models;
using AquaGuard.API.Models.Enums;
using AquaGuard.API.Repositories;
using AquaGuard.API.Services;
using AquaGuard.API.ViewModels.Leituras;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AquaGuard.Tests.Services;

public class LeituraServiceTests
{
    private static AppDbContext CriarContextoEmMemoria()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ConsumoNormal_NaoDeveGerarAlertaDeVazamento()
    {
        AppDbContext context = CriarContextoEmMemoria();

        Medidor medidor = new Medidor
        {
            Codigo = "MED-LEITURA-01",
            Localizacao = "Setor Testes",
            LimiteMensalLitros = 100000,
            Ativo = true
        };

        context.Medidores.Add(medidor);
        await context.SaveChangesAsync();

        MedidorRepository medidorRepository = new MedidorRepository(context);
        LeituraRepository leituraRepository = new LeituraRepository(context);
        AlertaRepository alertaRepository = new AlertaRepository(context);

        LeituraService service = new LeituraService(leituraRepository, medidorRepository, alertaRepository);

        CreateLeituraViewModel model = new CreateLeituraViewModel
        {
            MedidorId = medidor.Id,
            LitrosConsumidos = 500,
            DataLeitura = DateTime.UtcNow
        };

        LeituraResponseViewModel resultado = await service.CreateAsync(model, usuarioId: 1);

        Assert.NotNull(resultado);
        Assert.Equal(500, resultado.LitrosConsumidos);

        IEnumerable<Alerta> alertasGerados = await alertaRepository.GetAllAsync();
        Assert.Empty(alertasGerados);
    }

    [Fact]
    public async Task CreateAsync_ConsumoAcimaDe150PorcentoDaMedia_DeveGerarAlertaDeVazamento()
    {
        AppDbContext context = CriarContextoEmMemoria();
            
        Medidor medidor = new Medidor
        {
            Codigo = "MED-LEITURA-02",
            Localizacao = "Setor Testes",
            LimiteMensalLitros = 100000,
            Ativo = true
        };

        context.Medidores.Add(medidor);
        await context.SaveChangesAsync();

        // DEBUG
        Assert.True(medidor.Id > 0, $"Medidor.Id deveria ser > 0, mas é {medidor.Id}");

        context.Leituras.Add(new LeituraConsumo
        {
            MedidorId = medidor.Id,
            LitrosConsumidos = 1000,
            DataLeitura = DateTime.UtcNow.AddDays(-1),
            UsuarioId = 1
        });

        await context.SaveChangesAsync();

        MedidorRepository medidorRepository = new MedidorRepository(context);
        LeituraRepository leituraRepository = new LeituraRepository(context);
        AlertaRepository alertaRepository = new AlertaRepository(context);

        // DEBUG
        IEnumerable<LeituraConsumo> ultimosDias = await leituraRepository.GetUltimosDiasAsync(medidor.Id, 7);
        Assert.True(ultimosDias.Any(), $"GetUltimosDiasAsync retornou {ultimosDias.Count()} registros, esperava pelo menos 1");

        LeituraService service = new LeituraService(leituraRepository, medidorRepository, alertaRepository);

        CreateLeituraViewModel model = new CreateLeituraViewModel
        {
            MedidorId = medidor.Id,
            LitrosConsumidos = 1600,
            DataLeitura = DateTime.UtcNow
        };

        await service.CreateAsync(model, usuarioId: 1);

        IEnumerable<Alerta> alertasGerados = await alertaRepository.GetAllAsync();
        Assert.Single(alertasGerados);
        Assert.Equal(TipoAlerta.Vazamento, alertasGerados.First().Tipo);
    }
}