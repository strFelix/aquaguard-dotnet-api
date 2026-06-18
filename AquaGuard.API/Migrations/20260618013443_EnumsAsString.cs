using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquaGuard.API.Migrations
{
    /// <inheritdoc />
    public partial class EnumsAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resolvido",
                table: "Alertas");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Alertas",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Alertas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "SenhaHash",
                value: "$2a$11$UqHxm4TEWJnapK3OYO.TFebvxIX4GwqqTreA6aeDBvI3YTq3llliC");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "SenhaHash",
                value: "$2a$11$S/IXoxQZvha8aJEw8gbLO.2KBPe1TIiPF2bPvpcxHy4dPo1AlAZPS");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "DataCriacao", "Email", "Nome", "Role", "SenhaHash" },
                values: new object[] { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "auditor@aquaguard.com", "Auditor Padrão", "Auditor", "$2a$11$LKHsuckj4ukwS1JFN75qv.piZQ4EvcylSes3MIiSEVesrXLssEPlC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Alertas");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Alertas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<bool>(
                name: "Resolvido",
                table: "Alertas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "SenhaHash",
                value: "$2a$11$xmOJr/L3WHoUMfQGSmPTjOl1o3e.j4NAThCeGhTNiBvFp4TzqUItm");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "SenhaHash",
                value: "$2a$11$25UNlyV39xsRWaGcISxa5.yAKGBngF875XS5CQsqt0U4KZmZOZJwu");
        }
    }
}
