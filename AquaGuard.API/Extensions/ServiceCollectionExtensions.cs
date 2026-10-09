using AquaGuard.API.Data;
using AquaGuard.API.Repositories;
using AquaGuard.API.Repositories.Interfaces;
using AquaGuard.API.Services;
using AquaGuard.API.Services.Interfaces;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace AquaGuard.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IMedidorRepository, MedidorRepository>();
        services.AddScoped<ILeituraRepository, LeituraRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMedidorService, MedidorService>();
        services.AddScoped<ILeituraService, LeituraService>();
        services.AddScoped<IAlertaService, AlertaService>();
        services.AddScoped<IRelatorioService, RelatorioService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }

    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();

        return services;
    }

    public static IServiceCollection AddHealthChecksConfigured(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(name: "database", tags: new[] { "db" });

        return services;
    }
}