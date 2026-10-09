using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AquaGuard.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medidores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Localizacao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LimiteMensalLitros = table.Column<double>(type: "double precision", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medidores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedidorId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_Medidores_MedidorId",
                        column: x => x.MedidorId,
                        principalTable: "Medidores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leituras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MedidorId = table.Column<int>(type: "integer", nullable: false),
                    LitrosConsumidos = table.Column<double>(type: "double precision", nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leituras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leituras_Medidores_MedidorId",
                        column: x => x.MedidorId,
                        principalTable: "Medidores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leituras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Medidores",
                columns: new[] { "Id", "Ativo", "Codigo", "DataCriacao", "LimiteMensalLitros", "Localizacao" },
                values: new object[,]
                {
                    { 1, true, "MED-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 50000.0, "Bloco A - Térreo" },
                    { 2, true, "MED-002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30000.0, "Bloco B - 1º Andar" },
                    { 3, true, "MED-003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20000.0, "Área Externa - Jardim" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "DataCriacao", "Email", "Nome", "Role", "SenhaHash" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@aquaguard.com", "Administrador", "Admin", "$2a$11$6/.Szq6Zi.x1Em4clcVHmea9ENrFFzF9rtlOYtxj5lKLa14p5cmMK" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "operador@aquaguard.com", "Operador Padrão", "Operador", "$2a$11$lRkmrD5HEQsrdK6NhSnO5u4/pOoBeqw9NB8Z6e9qLadhhhrNUdEGu" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "auditor@aquaguard.com", "Auditor Padrão", "Auditor", "$2a$11$OlJAUVAYquiE0C5TWFmI9uzK6QKZTV8FRwOp0pIFYAns6Uqrcdln6" }
                });

            migrationBuilder.InsertData(
                table: "Leituras",
                columns: new[] { "Id", "DataLeitura", "LitrosConsumidos", "MedidorId", "UsuarioId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 1, 8, 0, 0, 0, DateTimeKind.Utc), 1200.0, 1, 2 },
                    { 2, new DateTime(2024, 5, 2, 8, 0, 0, 0, DateTimeKind.Utc), 1350.0, 1, 2 },
                    { 3, new DateTime(2024, 5, 1, 8, 0, 0, 0, DateTimeKind.Utc), 900.0, 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_MedidorId",
                table: "Alertas",
                column: "MedidorId");

            migrationBuilder.CreateIndex(
                name: "IX_Leituras_MedidorId",
                table: "Leituras",
                column: "MedidorId");

            migrationBuilder.CreateIndex(
                name: "IX_Leituras_UsuarioId",
                table: "Leituras",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Medidores_Codigo",
                table: "Medidores",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "Leituras");

            migrationBuilder.DropTable(
                name: "Medidores");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
