using AquaGuard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AquaGuard.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Medidor> Medidores { get; set; }
    public DbSet<LeituraConsumo> Leituras { get; set; }
    public DbSet<Alerta> Alertas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Nome).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(150);
            e.Property(u => u.SenhaHash).IsRequired();
            e.Property(u => u.Role).IsRequired().HasMaxLength(20);
        });

        // Medidor
        modelBuilder.Entity<Medidor>(e =>
        {
            e.HasKey(m => m.Id);
            e.HasIndex(m => m.Codigo).IsUnique();
            e.Property(m => m.Codigo).IsRequired().HasMaxLength(50);
            e.Property(m => m.Localizacao).IsRequired().HasMaxLength(200);
            e.Property(m => m.LimiteMensalLitros).IsRequired();
        });

        // LeituraConsumo
        modelBuilder.Entity<LeituraConsumo>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.LitrosConsumidos).IsRequired();
            e.Property(l => l.DataLeitura).IsRequired();

            e.HasOne(l => l.Medidor)
             .WithMany(m => m.Leituras)
             .HasForeignKey(l => l.MedidorId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.Usuario)
             .WithMany()
             .HasForeignKey(l => l.UsuarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Alerta
        modelBuilder.Entity<Alerta>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Tipo).IsRequired().HasMaxLength(50);
            e.Property(a => a.Descricao).IsRequired().HasMaxLength(500);

            e.HasOne(a => a.Medidor)
             .WithMany(m => m.Alertas)
             .HasForeignKey(a => a.MedidorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nome = "Administrador",
                Email = "admin@aquaguard.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Usuario
            {
                Id = 2,
                Nome = "Operador Padrão",
                Email = "operador@aquaguard.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Operador@123"),
                Role = "Operador",
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Medidor>().HasData(
            new Medidor
            {
                Id = 1,
                Codigo = "MED-001",
                Localizacao = "Bloco A - Térreo",
                LimiteMensalLitros = 50000,
                Ativo = true,
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Medidor
            {
                Id = 2,
                Codigo = "MED-002",
                Localizacao = "Bloco B - 1º Andar",
                LimiteMensalLitros = 30000,
                Ativo = true,
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Medidor
            {
                Id = 3,
                Codigo = "MED-003",
                Localizacao = "Área Externa - Jardim",
                LimiteMensalLitros = 20000,
                Ativo = true,
                DataCriacao = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<LeituraConsumo>().HasData(
            new LeituraConsumo
            {
                Id = 1,
                MedidorId = 1,
                LitrosConsumidos = 1200,
                DataLeitura = new DateTime(2024, 5, 1, 8, 0, 0, DateTimeKind.Utc),
                UsuarioId = 2
            },
            new LeituraConsumo
            {
                Id = 2,
                MedidorId = 1,
                LitrosConsumidos = 1350,
                DataLeitura = new DateTime(2024, 5, 2, 8, 0, 0, DateTimeKind.Utc),
                UsuarioId = 2
            },
            new LeituraConsumo
            {
                Id = 3,
                MedidorId = 2,
                LitrosConsumidos = 900,
                DataLeitura = new DateTime(2024, 5, 1, 8, 0, 0, DateTimeKind.Utc),
                UsuarioId = 2
            }
        );
    }
}