using AquaGuard.API.Data;
using AquaGuard.API.Repositories;
using AquaGuard.API.Services;
using AquaGuard.API.ViewModels.Medidores;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AquaGuard.Tests.Services;

public class MedidorServiceTests
{
    private static AppDbContext CriarContextoEmMemoria()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_DeveCriarMedidorComSucesso()
    {
        AppDbContext context = CriarContextoEmMemoria();
        MedidorRepository repository = new MedidorRepository(context);
        MedidorService service = new MedidorService(repository);

        CreateMedidorViewModel model = new CreateMedidorViewModel
        {
            Codigo = "MED-TESTE-01",
            Localizacao = "Setor de Testes",
            LimiteMensalLitros = 10000
        };

        MedidorResponseViewModel resultado = await service.CreateAsync(model);

        Assert.NotNull(resultado);
        Assert.Equal("MED-TESTE-01", resultado.Codigo);
        Assert.True(resultado.Ativo);
    }

    [Fact]
    public async Task DeactivateAsync_DeveDesativarMedidorExistente()
    {
        AppDbContext context = CriarContextoEmMemoria();
        MedidorRepository repository = new MedidorRepository(context);
        MedidorService service = new MedidorService(repository);

        CreateMedidorViewModel model = new CreateMedidorViewModel
        {
            Codigo = "MED-TESTE-02",
            Localizacao = "Setor de Testes",
            LimiteMensalLitros = 5000
        };

        MedidorResponseViewModel criado = await service.CreateAsync(model);
        bool resultado = await service.DeactivateAsync(criado.Id);

        Assert.True(resultado);

        MedidorResponseViewModel? medidorDesativado = await service.GetByIdAsync(criado.Id);
        Assert.NotNull(medidorDesativado);
        Assert.False(medidorDesativado!.Ativo);
    }
}